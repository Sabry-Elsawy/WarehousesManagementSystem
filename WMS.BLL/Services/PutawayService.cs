using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WMS.BLL.DTOs;
using WMS.BLL.Interfaces;
using WMS.DAL;
using WMS.DAL.Entities;
using WMS.DAL.UnitOfWork;

namespace WMS.BLL.Services
{
    public class PutawayService : IPutawayService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<PutawayService> _logger;

        public PutawayService(IUnitOfWork unitOfWork, ILogger<PutawayService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        #region New Methods

        public async Task<PutawayDto> CreatePutawayForReceiptItemAsync(int receiptItemId, int qty, string createdBy)
        {
            try
            {
                _logger.LogInformation("Creating putaway for ReceiptItem {ReceiptItemId}, Qty: {Qty}", receiptItemId, qty);

                var receiptItemRepo = _unitOfWork.GetRepository<ReceiptItem, int>();
                var receiptItem = await receiptItemRepo.GetByIdAsync(receiptItemId,
                    include: q => q.Include(ri => ri.Product));

                if (receiptItem == null)
                    throw new ArgumentException($"Receipt item {receiptItemId} not found");

                // Calculate available quantity (QtyReceived - already assigned to putaways)
                var putawayRepo = _unitOfWork.GetRepository<Putaway, int>();
                var existingPutaways = await putawayRepo.GetAllWithIncludeAsync(
                    withTracking: false,
                    include: q => q.Where(p => p.ReceiptItemId == receiptItemId));

                int alreadyAssignedQty = existingPutaways.Sum(p => p.Qty);
                int availableQty = receiptItem.QtyReceived - alreadyAssignedQty;

                if (qty > availableQty)
                    throw new ArgumentException($"Requested quantity {qty} exceeds available quantity {availableQty}");

                if (qty <= 0)
                    throw new ArgumentException("Quantity must be greater than 0");

                var putaway = new Putaway
                {
                    ReceiptItemId = receiptItemId,
                    Qty = qty,
                    Status = PutawayStatus.Pending,
                    CreatedBy = createdBy,
                    CreatedOn = DateTime.UtcNow
                };

                await putawayRepo.AddAsync(putaway);
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation("Created putaway {PutawayId} for ReceiptItem {ReceiptItemId}", putaway.Id, receiptItemId);

                return new PutawayDto
                {
                    Id = putaway.Id,
                    Qty = putaway.Qty,
                    Status = putaway.Status,
                    CreatedOn = putaway.CreatedOn,
                    CreatedBy = putaway.CreatedBy,
                    ReceiptItemId = receiptItemId,
                    SKU = receiptItem.SKU,
                    ProductName = receiptItem.Product?.Name ?? "",
                    ProductId = receiptItem.ProductId
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating putaway for ReceiptItem {ReceiptItemId}", receiptItemId);
                throw;
            }
        }

        public async Task<AutoAssignResultDto> AutoAssignBinsAsync(int putawayId)
        {
            try
            {
                _logger.LogInformation("Auto-assigning bins for Putaway {PutawayId}", putawayId);

                var putawayRepo = _unitOfWork.GetRepository<Putaway, int>();
                var putaway = await putawayRepo.GetByIdAsync(putawayId,
                    include: q => q.Include(p => p.ReceiptItem)
                                   .ThenInclude(ri => ri.Product));

                if (putaway == null)
                    throw new ArgumentException($"Putaway {putawayId} not found");

                if (putaway.Status != PutawayStatus.Pending && putaway.Status != PutawayStatus.Assigned)
                    throw new InvalidOperationException($"Cannot auto-assign bins for putaway with status {putaway.Status}");

                // Get all bins with available capacity
                var binRepo = _unitOfWork.GetRepository<Bin, int>();
                var inventoryRepo = _unitOfWork.GetRepository<Inventory, int>();

                var bins = await binRepo.GetAllWithIncludeAsync(false, q => q
                    .Include(b => b.BinType)
                    .Include(b => b.Rack)
                        .ThenInclude(r => r.Aisle)
                        .ThenInclude(a => a.Zone)
                    .Include(b => b.Inventories));

                var binCapacities = bins.Select(b => new BinCapacityDto
                {
                    BinId = b.Id,
                    Code = b.Code,
                    TotalCapacity = b.Capacity,
                    UsedCapacity = b.Inventories.Sum(i => i.Quantity),
                    BinTypeName = b.BinType?.Name ?? "",
                    BinTypeId = b.BinTypeId,
                    ZoneName = b.Rack?.Aisle?.Zone?.Name ?? "",
                    ZoneId = b.Rack?.Aisle?.Zone?.Id ?? 0,
                    CurrentProductQty = b.Inventories.Where(i => i.ProductId == putaway.ReceiptItem.ProductId).Sum(i => i.Quantity)
                }).Where(bc => bc.AvailableCapacity > 0) // Only bins with available capacity
                  .ToList();

                // Apply business rules for bin selection
                // Rule 1: Prefer bins with same product (co-location)
                // Rule 2: Sort by zone (lower zone id = higher priority, can be customized)
                // Rule 3: Sort by available capacity (descending) to minimize bin usage

                var sortedBins = binCapacities
                    .OrderByDescending(bc => bc.CurrentProductQty > 0) // Same product first
                    .ThenBy(bc => bc.ZoneId) // Zone priority
                    .ThenByDescending(bc => bc.AvailableCapacity) // Larger bins first
                    .ToList();

                // Greedy algorithm: fill bins until quantity is satisfied
                var suggestedAssignments = new List<PutawayBinDto>();
                int remainingQty = putaway.Qty;

                foreach (var bin in sortedBins)
                {
                    if (remainingQty <= 0) break;

                    int qtyToAssign = Math.Min(remainingQty, bin.AvailableCapacity);
                    
                    suggestedAssignments.Add(new PutawayBinDto
                    {
                        BinId = bin.BinId,
                        BinCode = bin.Code,
                        Qty = qtyToAssign,
                        AvailableCapacity = bin.AvailableCapacity,
                        BinType = bin.BinTypeName,
                        Zone = bin.ZoneName
                    });

                    remainingQty -= qtyToAssign;
                }

                int assignedQty = putaway.Qty - remainingQty;
                bool fullyAssigned = remainingQty == 0;

                // If fully assigned, update status to Assigned
                if (fullyAssigned)
                {
                    putaway.Status = PutawayStatus.Assigned;
                    putaway.AssignedOn = DateTime.UtcNow;
                    putawayRepo.Update(putaway);
                    await _unitOfWork.CompleteAsync();
                }

                var result = new AutoAssignResultDto
                {
                    PutawayId = putawayId,
                    TotalQty = putaway.Qty,
                    AssignedQty = assignedQty,
                    SuggestedBins = suggestedAssignments,
                    WarningMessage = fullyAssigned ? null : $"Could not fully assign. {remainingQty} units remaining. Insufficient bin capacity."
                };

                _logger.LogInformation("Auto-assign completed for Putaway {PutawayId}. Assigned: {AssignedQty}/{TotalQty}", 
                    putawayId, assignedQty, putaway.Qty);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error auto-assigning bins for Putaway {PutawayId}", putawayId);
                throw;
            }
        }

        public async Task AssignBinsManualAsync(int putawayId, IEnumerable<PutawayBinDto> assignments, string assignedBy)
        {
            try
            {
                _logger.LogInformation("Manually assigning bins for Putaway {PutawayId}", putawayId);

                var putawayRepo = _unitOfWork.GetRepository<Putaway, int>();
                var putaway = await putawayRepo.GetByIdAsync(putawayId);

                if (putaway == null)
                    throw new ArgumentException($"Putaway {putawayId} not found");

                if (putaway.Status != PutawayStatus.Pending && putaway.Status != PutawayStatus.Assigned)
                    throw new InvalidOperationException($"Cannot assign bins for putaway with status {putaway.Status}");

                var assignmentsList = assignments.ToList();

                // Validate total quantity matches
                int totalAssignedQty = assignmentsList.Sum(a => a.Qty);
                if (totalAssignedQty != putaway.Qty)
                    throw new ArgumentException($"Total assigned quantity {totalAssignedQty} does not match putaway quantity {putaway.Qty}");

                // Validate each bin has sufficient capacity
                var binRepo = _unitOfWork.GetRepository<Bin, int>();
                var inventoryRepo = _unitOfWork.GetRepository<Inventory, int>();

                foreach (var assignment in assignmentsList)
                {
                    var bin = await binRepo.GetByIdAsync(assignment.BinId);
                    if (bin == null)
                        throw new ArgumentException($"Bin {assignment.BinId} not found");

                    var inventories = await inventoryRepo.GetAllWithIncludeAsync(false,
                        include: q => q.Where(i => i.BinId == assignment.BinId));

                    int usedCapacity = inventories.Sum(i => i.Quantity);
                    int availableCapacity = bin.Capacity - usedCapacity;

                    if (assignment.Qty > availableCapacity)
                        throw new ArgumentException($"Bin {bin.Code} does not have sufficient capacity. Available: {availableCapacity}, Requested: {assignment.Qty}");
                }

                // Remove existing bin assignments
                var putawayBinRepo = _unitOfWork.GetRepository<PutawayBin, int>();
                var existingAssignments = await putawayBinRepo.GetAllWithIncludeAsync(true,
                    include: q => q.Where(pb => pb.PutawayId == putawayId));

                foreach (var existing in existingAssignments)
                {
                    putawayBinRepo.Delete(existing);
                }

                // Add new assignments
                foreach (var assignment in assignmentsList)
                {
                    var putawayBin = new PutawayBin
                    {
                        PutawayId = putawayId,
                        BinId = assignment.BinId,
                        Qty = assignment.Qty,
                        CreatedBy = assignedBy,
                        CreatedOn = DateTime.UtcNow
                    };

                    await putawayBinRepo.AddAsync(putawayBin);
                }

                // Update putaway status
                putaway.Status = PutawayStatus.Assigned;
                putaway.AssignedOn = DateTime.UtcNow;
                putaway.LastModifiedBy = assignedBy;
                putaway.LastModifiedOn = DateTime.UtcNow;
                putawayRepo.Update(putaway);

                await _unitOfWork.CompleteAsync();

                _logger.LogInformation("Successfully assigned bins manually for Putaway {PutawayId}", putawayId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error manually assigning bins for Putaway {PutawayId}", putawayId);
                throw;
            }
        }

        public async Task ExecutePutawayAsync(int putawayId, string performedBy)
        {
            try
            {
                _logger.LogInformation("Executing putaway {PutawayId} by {PerformedBy}", putawayId, performedBy);

                var putawayRepo = _unitOfWork.GetRepository<Putaway, int>();
                var putaway = await putawayRepo.GetByIdAsync(putawayId,
                    include: q => q.Include(p => p.ReceiptItem)
                                   .ThenInclude(ri => ri.ASNItem)
                                   .Include(p => p.PutawayBins));

                if (putaway == null)
                    throw new ArgumentException($"Putaway {putawayId} not found");

                if (putaway.Status != PutawayStatus.Assigned && putaway.Status != PutawayStatus.InProgress)
                    throw new InvalidOperationException($"Cannot execute putaway with status {putaway.Status}. Must be Assigned or InProgress.");

                if (!putaway.PutawayBins.Any())
                    throw new InvalidOperationException("No bins assigned to this putaway");

                // Update status to InProgress to prevent concurrent execution
                putaway.Status = PutawayStatus.InProgress;
                putawayRepo.Update(putaway);
                await _unitOfWork.CompleteAsync(); // Save status change immediately

                // Process each bin assignment
                var inventoryRepo = _unitOfWork.GetRepository<Inventory, int>();
                var binRepo = _unitOfWork.GetRepository<Bin, int>();

                foreach (var putawayBin in putaway.PutawayBins)
                {
                    // Verify bin still has capacity
                    var bin = await binRepo.GetByIdAsync(putawayBin.BinId);
                    if (bin == null)
                        throw new InvalidOperationException($"Bin {putawayBin.BinId} not found");

                    var binInventories = await inventoryRepo.GetAllWithIncludeAsync(false,
                        include: q => q.Where(i => i.BinId == putawayBin.BinId));

                    int usedCapacity = binInventories.Sum(i => i.Quantity);
                    int availableCapacity = bin.Capacity - usedCapacity;

                    if (putawayBin.Qty > availableCapacity)
                        throw new InvalidOperationException($"Bin {bin.Code} no longer has sufficient capacity");

                    // Check if inventory already exists for this product in this bin
                    var existingInventory = binInventories.FirstOrDefault(i => i.ProductId == putaway.ReceiptItem.ProductId);

                    if (existingInventory != null)
                    {
                        // Increment existing inventory
                        existingInventory.Quantity += putawayBin.Qty;
                        existingInventory.LastModifiedBy = performedBy;
                        existingInventory.LastModifiedOn = DateTime.UtcNow;
                        inventoryRepo.Update(existingInventory);

                        _logger.LogInformation("Incremented inventory for Product {ProductId} in Bin {BinId} by {Qty}",
                            putaway.ReceiptItem.ProductId, putawayBin.BinId, putawayBin.Qty);
                    }
                    else
                    {
                        // Create new inventory record
                        var inventory = new Inventory
                        {
                            ProductId = putaway.ReceiptItem.ProductId,
                            BinId = putawayBin.BinId,
                            Quantity = putawayBin.Qty,
                            Status = "Available",
                            BatchNumber = "BATCH-" + DateTime.Now.ToString("yyyyMMdd"),
                            ExpiryDate = "",
                            CreatedBy = performedBy,
                            CreatedOn = DateTime.UtcNow
                        };

                        await inventoryRepo.AddAsync(inventory);

                        _logger.LogInformation("Created new inventory for Product {ProductId} in Bin {BinId} with Qty {Qty}",
                            putaway.ReceiptItem.ProductId, putawayBin.BinId, putawayBin.Qty);
                    }
                }

                // Update putaway to completed
                putaway.Status = PutawayStatus.Completed;
                putaway.CompletedOn = DateTime.UtcNow;
                putaway.PerformedBy = performedBy;
                putaway.LastModifiedBy = performedBy;
                putaway.LastModifiedOn = DateTime.UtcNow;
                putawayRepo.Update(putaway);

                await _unitOfWork.CompleteAsync();

                _logger.LogInformation("Successfully executed putaway {PutawayId}", putawayId);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogWarning(ex, "Concurrency conflict executing putaway {PutawayId}", putawayId);
                throw new InvalidOperationException("This putaway has been modified by another user. Please refresh and try again.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing putaway {PutawayId}", putawayId);
                throw;
            }
        }

        public async Task ClosePutawayAsync(int putawayId, string closedBy)
        {
            try
            {
                _logger.LogInformation("Closing putaway {PutawayId} by {ClosedBy}", putawayId, closedBy);

                var putawayRepo = _unitOfWork.GetRepository<Putaway, int>();
                var putaway = await putawayRepo.GetByIdAsync(putawayId);

                if (putaway == null)
                    throw new ArgumentException($"Putaway {putawayId} not found");

                if (putaway.Status != PutawayStatus.Completed)
                    throw new InvalidOperationException($"Cannot close putaway with status {putaway.Status}. Must be Completed.");

                putaway.Status = PutawayStatus.Closed;
                putaway.ClosedOn = DateTime.UtcNow;
                putaway.ClosedBy = closedBy;
                putaway.LastModifiedBy = closedBy;
                putaway.LastModifiedOn = DateTime.UtcNow;

                putawayRepo.Update(putaway);
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation("Successfully closed putaway {PutawayId}", putawayId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error closing putaway {PutawayId}", putawayId);
                throw;
            }
        }

        public async Task<(IEnumerable<PutawayDto> Items, int TotalCount)> GetPagedPutawaysAsync(
            int pageNumber, int pageSize, string? searchTerm, string? statusFilter)
        {
            try
            {
                var putawayRepo = _unitOfWork.GetRepository<Putaway, int>();

                // Build filter expression
                System.Linq.Expressions.Expression<Func<Putaway, bool>>? filter = null;

                if (!string.IsNullOrWhiteSpace(searchTerm) || !string.IsNullOrWhiteSpace(statusFilter))
                {
                    filter = p =>
                        (string.IsNullOrWhiteSpace(searchTerm) ||
                         p.ReceiptItem.SKU.Contains(searchTerm) ||
                         p.ReceiptItem.Product.Name.Contains(searchTerm)) &&
                        (string.IsNullOrWhiteSpace(statusFilter) ||
                         p.Status.ToString() == statusFilter);
                }

                var (items, totalCount) = await putawayRepo.GetPagedListAsync(
                    pageNumber,
                    pageSize,
                    filter,
                    orderBy: q => q.OrderByDescending(p => p.CreatedOn),
                    includeProperties: "ReceiptItem,ReceiptItem.Product");

                var dtos = items.Select(p => new PutawayDto
                {
                    Id = p.Id,
                    Qty = p.Qty,
                    Status = p.Status,
                    CreatedOn = p.CreatedOn,
                    AssignedOn = p.AssignedOn,
                    CompletedOn = p.CompletedOn,
                    ClosedOn = p.ClosedOn,
                    CreatedBy = p.CreatedBy,
                    PerformedBy = p.PerformedBy,
                    ClosedBy = p.ClosedBy,
                    ReceiptItemId = p.ReceiptItemId,
                    SKU = p.ReceiptItem?.SKU ?? "",
                    ProductName = p.ReceiptItem?.Product?.Name ?? "",
                    ProductId = p.ReceiptItem?.ProductId ?? 0
                }).ToList();

                return (dtos, totalCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting paged putaways");
                throw;
            }
        }

        public async Task<PutawayDto?> GetPutawayDetailsAsync(int id)
        {
            try
            {
                var putawayRepo = _unitOfWork.GetRepository<Putaway, int>();
                var putaway = await putawayRepo.GetByIdAsync(id,
                    include: q => q.Include(p => p.ReceiptItem)
                                   .ThenInclude(ri => ri.Product)
                                   .Include(p => p.ReceiptItem)
                                   .ThenInclude(ri => ri.ASNItem)
                                   .Include(p => p.PutawayBins)
                                   .ThenInclude(pb => pb.Bin)
                                   .ThenInclude(b => b.Rack)
                                   .ThenInclude(r => r.Aisle)
                                   .ThenInclude(a => a.Zone));

                if (putaway == null)
                    return null;

                // Load inventory for each assigned bin
                var inventoryRepo = _unitOfWork.GetRepository<Inventory, int>();
                var assignedBins = new List<PutawayBinDto>();

                foreach (var pb in putaway.PutawayBins)
                {
                    var binInventories = await inventoryRepo.GetAllWithIncludeAsync(false,
                        include: q => q.Where(i => i.BinId == pb.BinId));

                    int usedCapacity = binInventories.Sum(i => i.Quantity);

                    assignedBins.Add(new PutawayBinDto
                    {
                        PutawayBinId = pb.Id,
                        BinId = pb.BinId,
                        BinCode = pb.Bin?.Code ?? "",
                        Qty = pb.Qty,
                        AvailableCapacity = (pb.Bin?.Capacity ?? 0) - usedCapacity,
                        Zone = pb.Bin?.Rack?.Aisle?.Zone?.Name ?? "",
                        BinType = pb.Bin?.BinType?.Name ?? ""
                    });
                }

                return new PutawayDto
                {
                    Id = putaway.Id,
                    Qty = putaway.Qty,
                    Status = putaway.Status,
                    CreatedOn = putaway.CreatedOn,
                    AssignedOn = putaway.AssignedOn,
                    CompletedOn = putaway.CompletedOn,
                    ClosedOn = putaway.ClosedOn,
                    CreatedBy = putaway.CreatedBy,
                    PerformedBy = putaway.PerformedBy,
                    ClosedBy = putaway.ClosedBy,
                    ReceiptItemId = putaway.ReceiptItemId,
                    SKU = putaway.ReceiptItem?.SKU ?? "",
                    ProductName = putaway.ReceiptItem?.Product?.Name ?? "",
                    ProductId = putaway.ReceiptItem?.ProductId ?? 0,
                    BatchNumber = null,
                    ExpiryDate = null,
                    AssignedBins = assignedBins
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting putaway details for {PutawayId}", id);
                throw;
            }
        }

        #endregion

        #region Legacy Methods (For Backwards Compatibility)

        public async Task<IEnumerable<Putaway>> GetAllAsync()
        {
            var repo = _unitOfWork.GetRepository<Putaway, int>();

            return await repo.GetAllWithIncludeAsync(
                withTracking: false,
                include: q => q
                    .Include(p => p.ReceiptItem)
                    .ThenInclude(ri => ri.Product)
            );
        }

        public async Task<Putaway?> GetByIdAsync(int id)
        {
            var repo = _unitOfWork.GetRepository<Putaway, int>();
            return await repo.GetByIdAsync(id,
                    include: q => q
                        .Include(p => p.ReceiptItem)
                        .ThenInclude(r => r.Product)
    );
        }

        public async Task<IEnumerable<BinDTO>> GetAvailableBinsAsync(int putawayId)
        {
            var putaway = await _unitOfWork.GetRepository<Putaway, int>()
                .GetByIdAsync(putawayId);

            if (putaway == null)
                return Enumerable.Empty<BinDTO>();

            var bins = await _unitOfWork.GetRepository<Bin, int>()
                .GetAllWithIncludeAsync(false, q => q.Include(b => b.Inventories));

            return bins.Select(b =>
            {
                var used = b.Inventories.Sum(i => i.Quantity);

                return new BinDTO
                {
                    BinId = b.Id,
                    Code = b.Code,
                    Capacity = b.Capacity,
                    Used = used
                };
            }).ToList();
        }


        public async Task<bool> AssignAsync(int putawayId, int binId, int qty)
        {
            var putawayRepo = _unitOfWork.GetRepository<Putaway, int>();
            var putaway = await putawayRepo.GetByIdAsync(putawayId);

            if (putaway == null)
                throw new InvalidOperationException("Putaway not found");

            var binRepo = _unitOfWork.GetRepository<Bin, int>();
            var bin = await binRepo.GetByIdAsync(binId);

            if (bin == null)
                throw new InvalidOperationException("Bin not found");

            var inventoryRepo = _unitOfWork.GetRepository<Inventory, int>();
            var inventoryItems = await inventoryRepo.GetAllAsync();
            var used = inventoryItems.Where(i => i.BinId == binId).Sum(i => i.Quantity);

            if (bin.Capacity - used < qty)
                throw new InvalidOperationException("Not enough bin capacity");

            var putawayBinRepo = _unitOfWork.GetRepository<PutawayBin, int>();
            var entry = new PutawayBin
            {
                PutawayId = putawayId,
                BinId = binId,
                Qty = qty
            };

            await putawayBinRepo.AddAsync(entry);
            await _unitOfWork.CompleteAsync();
            return true;
        }

        public async Task<bool> CompleteAsync(int putawayId)
        {
            var putawayRepo = _unitOfWork.GetRepository<Putaway, int>();
            var putaway = await putawayRepo.GetByIdAsync(putawayId);

            if (putaway == null)
                throw new InvalidOperationException("Putaway not found");

            var linksRepo = _unitOfWork.GetRepository<PutawayBin, int>();
            var links = await linksRepo.GetAllWithIncludeAsync(
                withTracking: false,
                include: q => q.Where(l => l.PutawayId == putawayId)
            );


            if (!links.Any())
                throw new InvalidOperationException("No bins assigned");

            var receiptItemRepo = _unitOfWork.GetRepository<ReceiptItem, int>();
            var receiptItem = await receiptItemRepo.GetByIdAsync(putaway.ReceiptItemId);

            if (receiptItem == null)
                throw new InvalidOperationException("Invalid receipt item");

            var inventoryRepo = _unitOfWork.GetRepository<Inventory, int>();

            foreach (var link in links)
            {
                var inv = new Inventory
                {
                    ProductId = receiptItem.ProductId,
                    BinId = link.BinId,
                    Quantity = link.Qty,
                    Status = "Available",
                    BatchNumber = "BATCH-001",
                    ExpiryDate = ""
                };

                await inventoryRepo.AddAsync(inv);
            }

            putaway.Status = PutawayStatus.Completed;
            putawayRepo.Update(putaway);

            await _unitOfWork.CompleteAsync();
            return true;
        }

        #endregion
    }
}

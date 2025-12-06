using Microsoft.EntityFrameworkCore;
using WMS.BLL.DTOs;
using WMS.BLL.Interfaces;
using WMS.DAL;
using WMS.DAL.Entities;
using WMS.DAL.UnitOfWork;

namespace WMS.BLL.Services;
public class InventoryService:IInventoryService
{
    private readonly IUnitOfWork _unitOfWork;

    public InventoryService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<InventoryListDto>> GetAllAsync(string? search = null)
    {
        var repo = _unitOfWork.GetRepository<Inventory, int>();

        var data = await repo.GetAllWithIncludeAsync(
            withTracking: false,
            include: q => q
                .Include(i => i.Product)
                .Include(i => i.Bin)
        );

        
        var list = data.Select(i => new InventoryListDto
        {
            Id = i.Id,
            SKU = i.Product.Code,
            ProductName = i.Product.Name,
            Quantity = i.Quantity,
            Location = i.Bin.Code,
            Status = i.Status,
            LastUpdated = i.LastModifiedOn ?? i.CreatedOn
        });

        
        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim().ToLower();

            list = list.Where(x =>
                   x.SKU.ToLower().Contains(s)
                || x.ProductName.ToLower().Contains(s)
                || x.Location.ToLower().Contains(s)
                || x.Status.ToLower().Contains(s)
            );
        }

        return list.OrderByDescending(x => x.LastUpdated).ToList();
    }

    public async Task<bool> AddInventoryAsync(AddInventoryDto dto)
    {
        
        var productRepo = _unitOfWork.GetRepository<Product, int>();
        var product = (await productRepo.GetAllAsync())
            .FirstOrDefault(p => p.Name.ToLower() == dto.ProductName.ToLower());

        if (product == null)
            throw new Exception("Product not found. Create product first.");

        
        var binRepo = _unitOfWork.GetRepository<Bin, int>();
        var bin = (await binRepo.GetAllAsync())
            .FirstOrDefault(b => b.Code.ToLower() == dto.Location.ToLower());

        if (bin == null)
            throw new Exception("Bin not found.");

        
        var inventoryRepo = _unitOfWork.GetRepository<Inventory, int>();

        var inv = new Inventory
        {
            ProductId = product.Id,
            BinId = bin.Id,
            Quantity = dto.Quantity,
            Status = dto.Status,
            CreatedOn = DateTime.Now,
            BatchNumber = "",
            ExpiryDate = ""
        };

        await inventoryRepo.AddAsync(inv);
        await _unitOfWork.CompleteAsync();

        // Log Transaction
        var transactionRepo = _unitOfWork.GetRepository<InventoryTransaction, int>();
        var transaction = new InventoryTransaction
        {
            TransactionType = "Receipt",
            QuantityChange = dto.Quantity,
            ProductId = product.Id,
            DestinationBinId = bin.Id,
            TransactionDate = DateTime.UtcNow,
            CreatedBy = "System", // TODO: Get current user
            ReferenceNumber = $"INV-ADD-{DateTime.UtcNow:yyyyMMddHHmmss}",
            Reason = "Initial Inventory Add"
        };
        await transactionRepo.AddAsync(transaction);
        await _unitOfWork.CompleteAsync();

        return true;
    }
    public async Task<bool> AdjustInventoryAsync(AdjustInventoryDto dto)
    {
        
        var productRepo = _unitOfWork.GetRepository<Product, int>();
        var product = (await productRepo.GetAllAsync())
            .FirstOrDefault(p => p.Code.ToLower() == dto.SKU.ToLower());

        if (product == null)
            throw new Exception("SKU not found.");

        
        var invRepo = _unitOfWork.GetRepository<Inventory, int>();
        var inventories = await invRepo.GetAllAsync();

        var inv = inventories.FirstOrDefault(i => i.ProductId == product.Id);

        if (inv == null)
            throw new Exception("No inventory found for this SKU.");

        
        int newQty = inv.Quantity + dto.Change;

        if (newQty < 0)
            throw new Exception("Quantity cannot be negative.");

        inv.Quantity = newQty;
        inv.LastModifiedOn = DateTime.Now;

        invRepo.Update(inv);
        await _unitOfWork.CompleteAsync();

        // Log Transaction
        var transactionRepo = _unitOfWork.GetRepository<InventoryTransaction, int>();
        var transaction = new InventoryTransaction
        {
            TransactionType = "Adjustment",
            QuantityChange = dto.Change,
            ProductId = product.Id,
            SourceBinId = inv.BinId, // Assuming adjustment happens in place
            DestinationBinId = inv.BinId,
            TransactionDate = DateTime.UtcNow,
            CreatedBy = dto.PerformedBy,
            ReferenceNumber = $"ADJ-{product.Code}-{DateTime.UtcNow:yyyyMMddHHmmss}",
            Reason = dto.Reason
        };
        await transactionRepo.AddAsync(transaction);
        await _unitOfWork.CompleteAsync();

        return true;
    }

    public async Task<InventoryDetailsDto?> GetByIdAsync(int id)
    {
        var repo = _unitOfWork.GetRepository<Inventory, int>();
        var inventory = await repo.GetByIdAsync(id, 
            query => query
                .Include(i => i.Product)
                .Include(i => i.Bin));

        if (inventory == null)
            return null;

        return new InventoryDetailsDto
        {
            Id = inventory.Id,
            ProductId = inventory.ProductId,
            SKU = inventory.Product.Code,
            ProductName = inventory.Product.Name,
            BinId = inventory.BinId,
            BinCode = inventory.Bin.Code,
            Quantity = inventory.Quantity,
            Status = inventory.Status,
            BatchNumber = inventory.BatchNumber,
            ExpiryDate = inventory.ExpiryDate,
            CreatedOn = inventory.CreatedOn,
            CreatedBy = inventory.CreatedBy,
            LastModifiedOn = inventory.LastModifiedOn,
            LastModifiedBy = inventory.LastModifiedBy,
            RecentTransactions = (await GetTransactionsAsync(inventory.ProductId, inventory.BinId))
                .Take(10)
                .ToList()
        };
    }

    public async Task<IEnumerable<InventoryListDto>> GetByProductIdAsync(int productId)
    {
        var repo = _unitOfWork.GetRepository<Inventory, int>();
        var inventories = await repo.GetAllWithIncludeAsync(
            withTracking: false,
            include: q => q
                .Include(i => i.Product)
                .Include(i => i.Bin)
                .Where(i => i.ProductId == productId));

        return inventories.Select(i => new InventoryListDto
        {
            Id = i.Id,
            SKU = i.Product.Code,
            ProductName = i.Product.Name,
            Quantity = i.Quantity,
            Location = i.Bin.Code,
            Status = i.Status,
            LastUpdated = i.LastModifiedOn ?? i.CreatedOn
        }).ToList();
    }

    public async Task<IEnumerable<InventoryListDto>> GetByBinIdAsync(int binId)
    {
        var repo = _unitOfWork.GetRepository<Inventory, int>();
        var inventories = await repo.GetAllWithIncludeAsync(
            withTracking: false,
            include: q => q
                .Include(i => i.Product)
                .Include(i => i.Bin)
                .Where(i => i.BinId == binId));

        return inventories.Select(i => new InventoryListDto
        {
            Id = i.Id,
            SKU = i.Product.Code,
            ProductName = i.Product.Name,
            Quantity = i.Quantity,
            Location = i.Bin.Code,
            Status = i.Status,
            LastUpdated = i.LastModifiedOn ?? i.CreatedOn
        }).ToList();
    }

    public async Task<(IEnumerable<InventoryListDto> Items, int TotalCount)> GetPagedAsync(
        int pageNumber, int pageSize,
        string? search, int? productId, int? binId, string? status)
    {
        var repo = _unitOfWork.GetRepository<Inventory, int>();
        var query = await repo.GetAllWithIncludeAsync(
            withTracking: false,
            include: q => q
                .Include(i => i.Product)
                .Include(i => i.Bin));

        // Apply filters
        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim().ToLower();
            query = query.Where(i =>
                i.Product.Code.ToLower().Contains(s) ||
                i.Product.Name.ToLower().Contains(s) ||
                i.Bin.Code.ToLower().Contains(s));
        }

        if (productId.HasValue)
            query = query.Where(i => i.ProductId == productId.Value);

        if (binId.HasValue)
            query = query.Where(i => i.BinId == binId.Value);

        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(i => i.Status.ToLower() == status.ToLower());

        var totalCount = query.Count();

        // Apply pagination
        var items = query
            .OrderByDescending(i => i.LastModifiedOn ?? i.CreatedOn)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(i => new InventoryListDto
            {
                Id = i.Id,
                SKU = i.Product.Code,
                ProductName = i.Product.Name,
                Quantity = i.Quantity,
                Location = i.Bin.Code,
                Status = i.Status,
                LastUpdated = i.LastModifiedOn ?? i.CreatedOn
            })
            .ToList();

        return (items, totalCount);
    }

    public async Task<bool> TransferInventoryAsync(TransferInventoryDto dto)
    {
        if (dto.Quantity <= 0)
            throw new ArgumentException("Transfer quantity must be greater than 0");

        var inventoryRepo = _unitOfWork.GetRepository<Inventory, int>();
        
        // Get source inventory with tracking
        var sourceInventory = await inventoryRepo.GetByIdAsync(dto.InventoryId);
        if (sourceInventory == null)
            throw new InvalidOperationException("Source inventory not found");

        if (sourceInventory.Quantity < dto.Quantity)
            throw new InvalidOperationException($"Insufficient quantity. Available: {sourceInventory.Quantity}, Requested: {dto.Quantity}");

        // Validate destination bin exists
        var binRepo = _unitOfWork.GetRepository<Bin, int>();
        var destinationBin = await binRepo.GetByIdAsync(dto.DestinationBinId);
        if (destinationBin == null)
            throw new InvalidOperationException("Destination bin not found");

        // Check if inventory already exists in destination bin
        var allInventories = await inventoryRepo.GetAllAsync(WithTracking: true);
        var destinationInventory = allInventories.FirstOrDefault(i =>
            i.ProductId == sourceInventory.ProductId &&
            i.BinId == dto.DestinationBinId);

        // Deduct from source
        sourceInventory.Quantity -= dto.Quantity;
        sourceInventory.LastModifiedOn = DateTime.UtcNow;
        sourceInventory.LastModifiedBy = dto.PerformedBy;
        inventoryRepo.Update(sourceInventory);

        // Add to destination
        if (destinationInventory != null)
        {
            // Update existing inventory in destination
            destinationInventory.Quantity += dto.Quantity;
            destinationInventory.LastModifiedOn = DateTime.UtcNow;
            destinationInventory.LastModifiedBy = dto.PerformedBy;
            inventoryRepo.Update(destinationInventory);
        }
        else
        {
            // Create new inventory record in destination
            var newInventory = new Inventory
            {
                ProductId = sourceInventory.ProductId,
                BinId = dto.DestinationBinId,
                Quantity = dto.Quantity,
                Status = sourceInventory.Status,
                BatchNumber = sourceInventory.BatchNumber,
                ExpiryDate = sourceInventory.ExpiryDate,
                CreatedOn = DateTime.UtcNow,
                CreatedBy = dto.PerformedBy
            };
            await inventoryRepo.AddAsync(newInventory);
        }

        await _unitOfWork.CompleteAsync();

        // Log Transaction
        var transactionRepo = _unitOfWork.GetRepository<InventoryTransaction, int>();
        var transaction = new InventoryTransaction
        {
            TransactionType = "Transfer",
            QuantityChange = dto.Quantity,
            ProductId = sourceInventory.ProductId,
            SourceBinId = sourceInventory.BinId,
            DestinationBinId = dto.DestinationBinId,
            TransactionDate = DateTime.UtcNow,
            CreatedBy = dto.PerformedBy,
            ReferenceNumber = $"TRF-{dto.InventoryId}-{DateTime.UtcNow:yyyyMMddHHmmss}",
            Reason = dto.Reason
        };
        await transactionRepo.AddAsync(transaction);
        await _unitOfWork.CompleteAsync();

        return true;
    }

    public async Task<Dictionary<string, int>> GetStockSummaryAsync()
    {
        var repo = _unitOfWork.GetRepository<Inventory, int>();
        var allInventories = await repo.GetAllAsync(WithTracking: false);

        var summary = allInventories
            .GroupBy(i => i.Status)
            .ToDictionary(
                g => g.Key,
                g => g.Sum(i => i.Quantity)
            );

        return summary;
    }

    public async Task<IEnumerable<InventoryTransactionDto>> GetTransactionsAsync(int? productId = null, int? binId = null)
    {
        var repo = _unitOfWork.GetRepository<InventoryTransaction, int>();
        var query = await repo.GetAllWithIncludeAsync(
            withTracking: false,
            include: q => q
                .Include(t => t.Product)
                .Include(t => t.SourceBin)
                .Include(t => t.DestinationBin)
        );

        if (productId.HasValue)
            query = query.Where(t => t.ProductId == productId.Value);

        if (binId.HasValue)
            query = query.Where(t => t.SourceBinId == binId.Value || t.DestinationBinId == binId.Value);

        return query.OrderByDescending(t => t.TransactionDate).Select(t => new InventoryTransactionDto
        {
            Id = t.Id,
            TransactionType = t.TransactionType,
            QuantityChange = t.QuantityChange,
            SourceBin = t.SourceBin?.Code,
            DestinationBin = t.DestinationBin?.Code,
            Reason = t.Reason,
            TransactionDate = t.TransactionDate,
            PerformedBy = t.CreatedBy
        }).ToList();
    }
}

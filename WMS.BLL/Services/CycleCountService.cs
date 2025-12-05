using Microsoft.EntityFrameworkCore;
using WMS.BLL.DTOs;
using WMS.BLL.Interfaces;
using WMS.DAL.Entities;
using WMS.DAL.UnitOfWork;

namespace WMS.BLL.Services;

public class CycleCountService : ICycleCountService
{
    private readonly IUnitOfWork _unitOfWork;

    public CycleCountService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<int> CreateSessionAsync(CreateCycleCountDto dto, string createdBy)
    {
        // 1. Create Session
        var session = new CycleCount
        {
            Status = "Pending",
            Description = dto.Description,
            CreatedBy = createdBy,
            CreatedOn = DateTime.UtcNow
        };

        var sessionRepo = _unitOfWork.GetRepository<CycleCount, int>();
        await sessionRepo.AddAsync(session);
        await _unitOfWork.CompleteAsync(); // Save to get ID

        // 2. Select Items based on filters
        var inventoryRepo = _unitOfWork.GetRepository<Inventory, int>();
        var query = await inventoryRepo.GetAllWithIncludeAsync(
            withTracking: false,
            include: q => q.Include(i => i.Product).Include(i => i.Bin)
        );

        if (dto.ProductIds != null && dto.ProductIds.Any())
            query = query.Where(i => dto.ProductIds.Contains(i.ProductId));

        if (dto.BinIds != null && dto.BinIds.Any())
            query = query.Where(i => dto.BinIds.Contains(i.BinId));

        if (dto.ZoneId.HasValue)
            query = query.Where(i => i.Bin.ZoneId == dto.ZoneId.Value);

        // 3. Create Cycle Count Items
        var itemRepo = _unitOfWork.GetRepository<CycleCountItem, int>();
        var items = query.Select(i => new CycleCountItem
        {
            CycleCountId = session.Id,
            ProductId = i.ProductId,
            BinId = i.BinId,
            ExpectedQuantity = i.Quantity,
            CountedQuantity = 0, // Default to 0 or Expected? Usually 0 or blind
            Status = "Pending",
            CreatedBy = createdBy,
            CreatedOn = DateTime.UtcNow
        }).ToList();

        foreach (var item in items)
        {
            await itemRepo.AddAsync(item);
        }

        await _unitOfWork.CompleteAsync();
        return session.Id;
    }

    public async Task<CycleCountDto?> GetSessionAsync(int id)
    {
        var repo = _unitOfWork.GetRepository<CycleCount, int>();
        var session = await repo.GetByIdAsync(id, q => q.Include(s => s.Items));

        if (session == null) return null;

        return new CycleCountDto
        {
            Id = session.Id,
            Status = session.Status,
            Description = session.Description,
            StartedAt = session.StartedAt,
            CompletedAt = session.CompletedAt,
            CreatedOn = session.CreatedOn,
            CreatedBy = session.CreatedBy,
            TotalItems = session.Items.Count,
            TotalDiscrepancies = session.Items.Count(i => i.CountedQuantity != i.ExpectedQuantity)
        };
    }

    public async Task<IEnumerable<CycleCountDto>> GetAllSessionsAsync()
    {
        var repo = _unitOfWork.GetRepository<CycleCount, int>();
        var sessions = await repo.GetAllWithIncludeAsync(
            withTracking: false,
            include: q => q.Include(s => s.Items)
        );

        return sessions.OrderByDescending(s => s.CreatedOn).Select(s => new CycleCountDto
        {
            Id = s.Id,
            Status = s.Status,
            Description = s.Description,
            StartedAt = s.StartedAt,
            CompletedAt = s.CompletedAt,
            CreatedOn = s.CreatedOn,
            CreatedBy = s.CreatedBy,
            TotalItems = s.Items.Count,
            TotalDiscrepancies = s.Items.Count(i => i.CountedQuantity != i.ExpectedQuantity)
        }).ToList();
    }

    public async Task<IEnumerable<CycleCountItemDto>> GetSessionItemsAsync(int sessionId)
    {
        var repo = _unitOfWork.GetRepository<CycleCountItem, int>();
        var items = await repo.GetAllWithIncludeAsync(
            withTracking: false,
            include: q => q
                .Include(i => i.Product)
                .Include(i => i.Bin)
                .Where(i => i.CycleCountId == sessionId)
        );

        return items.Select(i => new CycleCountItemDto
        {
            Id = i.Id,
            ProductId = i.ProductId,
            SKU = i.Product.Code,
            ProductName = i.Product.Name,
            BinId = i.BinId,
            BinCode = i.Bin.Code,
            ExpectedQuantity = i.ExpectedQuantity,
            CountedQuantity = i.CountedQuantity,
            Difference = i.Difference,
            Status = i.Status,
            Notes = i.Notes
        }).ToList();
    }

    public async Task<bool> UpdateCountAsync(UpdateCountDto dto)
    {
        var repo = _unitOfWork.GetRepository<CycleCountItem, int>();
        var item = await repo.GetByIdAsync(dto.ItemId);

        if (item == null) return false;

        item.CountedQuantity = dto.CountedQuantity;
        item.Notes = dto.Notes;
        item.Status = "Counted";
        item.LastModifiedOn = DateTime.UtcNow;

        repo.Update(item);
        await _unitOfWork.CompleteAsync();
        return true;
    }

    public async Task<bool> FinalizeSessionAsync(int sessionId, string performedBy)
    {
        var sessionRepo = _unitOfWork.GetRepository<CycleCount, int>();
        var session = await sessionRepo.GetByIdAsync(sessionId, q => q.Include(s => s.Items));

        if (session == null) return false;

        var inventoryRepo = _unitOfWork.GetRepository<Inventory, int>();
        var transactionRepo = _unitOfWork.GetRepository<InventoryTransaction, int>();
        var allInventories = await inventoryRepo.GetAllAsync(WithTracking: true);

        foreach (var item in session.Items)
        {
            if (item.CountedQuantity != item.ExpectedQuantity)
            {
                // Adjust Inventory
                var inv = allInventories.FirstOrDefault(i => i.ProductId == item.ProductId && i.BinId == item.BinId);
                if (inv != null)
                {
                    inv.Quantity = item.CountedQuantity;
                    inv.LastModifiedOn = DateTime.UtcNow;
                    inv.LastModifiedBy = performedBy;
                    inventoryRepo.Update(inv);

                    // Log Transaction
                    var transaction = new InventoryTransaction
                    {
                        TransactionType = "CycleCount",
                        QuantityChange = item.CountedQuantity - item.ExpectedQuantity,
                        ProductId = item.ProductId,
                        SourceBinId = item.BinId,
                        DestinationBinId = item.BinId,
                        TransactionDate = DateTime.UtcNow,
                        CreatedBy = performedBy,
                        Reason = $"Cycle Count Adjustment (Session #{sessionId})"
                    };
                    await transactionRepo.AddAsync(transaction);
                }
            }
            item.Status = "Approved";
        }

        session.Status = "Completed";
        session.CompletedAt = DateTime.UtcNow;
        sessionRepo.Update(session);

        await _unitOfWork.CompleteAsync();
        return true;
    }
}

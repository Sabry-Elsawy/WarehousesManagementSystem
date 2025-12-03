using Microsoft.EntityFrameworkCore;
using WMS.BLL.Interfaces;
using WMS.DAL;
using WMS.DAL.UnitOfWork;

namespace WMS.BLL.Services;

public class PickingService : IPickingService
{
    private readonly IUnitOfWork _unitOfWork;

    public PickingService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<Picking>> GetAllAsync()
    {
        var repo = _unitOfWork.GetRepository<Picking, int>();
        return await repo.GetAllWithIncludeAsync(withTracking: false, query => query
            .Include(p => p.SO_Item)
                .ThenInclude(i => i.SalesOrder)
                    .ThenInclude(s => s.Customer)
            .Include(p => p.Product)
            .Include(p => p.Bin));
    }

    public async Task<IEnumerable<Picking>> GetBySalesOrderIdAsync(int soId)
    {
        var repo = _unitOfWork.GetRepository<Picking, int>();
        var itemRepo = _unitOfWork.GetRepository<SO_Item, int>();

        // Get all items for this SO
        var items = await itemRepo.GetAllAsync(withTracking: false);
        var soItemIds = items.Where(i => i.SalesOrderId == soId).Select(i => i.Id).ToList();

        // Get all pickings for these items
        var allPickings = await repo.GetAllWithIncludeAsync(withTracking: false, query => query
            .Include(p => p.SO_Item)
            .Include(p => p.Product)
            .Include(p => p.Bin));

        return allPickings.Where(p => soItemIds.Contains(p.SO_ItemId));
    }

    public async Task<Picking?> GetByIdAsync(int id)
    {
        var repo = _unitOfWork.GetRepository<Picking, int>();
        return await repo.GetByIdAsync(
            id,
            include: q => q
                .Include(p => p.SO_Item)
                .Include(p => p.Product)
                .Include(p => p.Bin));
    }

    public async Task<bool> AllocatePickingTasksAsync(int salesOrderId)
    {
        var soRepo = _unitOfWork.GetRepository<SalesOrder, int>();
        var so = await soRepo.GetByIdAsync(
            salesOrderId,
            include: q => q.Include(s => s.SO_Items));

        if (so == null)
            throw new InvalidOperationException("Sales Order not found");

        if (so.Status != SalesOrderStatus.Submitted)
            throw new InvalidOperationException("Only Submitted Sales Orders can have picking tasks allocated");

        // Check if picking tasks already exist
        var existingPickings = await GetBySalesOrderIdAsync(salesOrderId);
        if (existingPickings.Any())
            throw new InvalidOperationException("Picking tasks already allocated for this Sales Order");

        var inventoryRepo = _unitOfWork.GetRepository<Inventory, int>();
        var pickingRepo = _unitOfWork.GetRepository<Picking, int>();
        var binRepo = _unitOfWork.GetRepository<Bin, int>();

        foreach (var soItem in so.SO_Items)
        {
            var qtyNeeded = soItem.QtyOrdered;

            // Get available inventory for this product, ordered by BinId (simple FIFO)
            var inventoryRecords = await inventoryRepo.GetAllAsync(withTracking: true);
            var availableInventory = inventoryRecords
                .Where(inv => inv.ProductId == soItem.ProductId && inv.Quantity > 0)
                .OrderBy(inv => inv.BinId)
                .ToList();

            if (!availableInventory.Any())
            {
                throw new InvalidOperationException($"No inventory available for product ID {soItem.ProductId}");
            }

            var totalAvailable = availableInventory.Sum(inv => inv.Quantity);
            if (totalAvailable < qtyNeeded)
            {
                throw new InvalidOperationException($"Insufficient inventory for product ID {soItem.ProductId}. Required: {qtyNeeded}, Available: {totalAvailable}");
            }

            // Allocate from bins
            var qtyRemaining = qtyNeeded;
            foreach (var invRecord in availableInventory)
            {
                if (qtyRemaining <= 0)
                    break;

                var qtyToAllocate = Math.Min(qtyRemaining, invRecord.Quantity);

                // Create picking task
                var pickingTask = new Picking
                {
                    SO_ItemId = soItem.Id,
                    ProductId = soItem.ProductId,
                    BinId = invRecord.BinId,
                    QuantityToPick = qtyToAllocate,
                    QuantityPicked = 0,
                    Status = PickingStatus.Pending
                };

                await pickingRepo.AddAsync(pickingTask);

                qtyRemaining -= qtyToAllocate;
            }
        }

        await _unitOfWork.CompleteAsync();
        return true;
    }

    public async Task<bool> ConfirmPickingAsync(int pickingId, int quantityPicked)
    {
        var pickingRepo = _unitOfWork.GetRepository<Picking, int>();
        var picking = await pickingRepo.GetByIdAsync(
            pickingId,
            include: q => q.Include(p => p.SO_Item)
                          .ThenInclude(i => i.SalesOrder));

        if (picking == null)
            throw new InvalidOperationException("Picking task not found");

        if (picking.Status == PickingStatus.Picked)
            throw new InvalidOperationException("This picking task has already been confirmed");

        if (picking.Status == PickingStatus.Cancelled)
            throw new InvalidOperationException("Cannot confirm a cancelled picking task");

        // Validation
        if (quantityPicked < 0)
            throw new ArgumentException("Quantity picked cannot be negative");

        if (quantityPicked > picking.QuantityToPick)
            throw new ArgumentException($"Quantity picked ({quantityPicked}) cannot exceed quantity to pick ({picking.QuantityToPick})");

        // Update picking task
        picking.QuantityPicked = quantityPicked;
        picking.Status = PickingStatus.Picked;
        pickingRepo.Update(picking);

        // Update SO_Item
        var itemRepo = _unitOfWork.GetRepository<SO_Item, int>();
        var soItem = await itemRepo.GetByIdAsync(picking.SO_ItemId);
        if (soItem != null)
        {
            soItem.QtyPicked += quantityPicked;
            itemRepo.Update(soItem);
        }

        // Reduce inventory
        var inventoryRepo = _unitOfWork.GetRepository<Inventory, int>();
        var inventoryRecords = await inventoryRepo.GetAllAsync(withTracking: true);
        var inventory = inventoryRecords.FirstOrDefault(inv => inv.BinId == picking.BinId && inv.ProductId == picking.ProductId);

        if (inventory != null)
        {
            inventory.Quantity -= quantityPicked;
            inventoryRepo.Update(inventory);
        }

        // Check if all picking tasks for this SO are completed
        var allPickingsForSO = await GetBySalesOrderIdAsync(picking.SO_Item.SalesOrderId);
        var allPicked = allPickingsForSO.All(p => p.Status == PickingStatus.Picked || p.Status == PickingStatus.Cancelled);

        if (allPicked)
        {
            // Update Sales Order status to Picked
            var soRepo = _unitOfWork.GetRepository<SalesOrder, int>();
            var so = await soRepo.GetByIdAsync(picking.SO_Item.SalesOrderId);
            if (so != null)
            {
                so.Status = SalesOrderStatus.Picked;
                soRepo.Update(so);
            }
        }

        await _unitOfWork.CompleteAsync();
        return true;
    }

    public async Task<bool> CancelPickingAsync(int pickingId)
    {
        var pickingRepo = _unitOfWork.GetRepository<Picking, int>();
        var picking = await pickingRepo.GetByIdAsync(pickingId);

        if (picking == null)
            throw new InvalidOperationException("Picking task not found");

        if (picking.Status == PickingStatus.Picked)
            throw new InvalidOperationException("Cannot cancel a picked task");

        if (picking.Status == PickingStatus.Cancelled)
            throw new InvalidOperationException("This picking task is already cancelled");

        picking.Status = PickingStatus.Cancelled;
        pickingRepo.Update(picking);
        await _unitOfWork.CompleteAsync();

        return true;
    }
}

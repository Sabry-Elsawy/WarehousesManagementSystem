using Microsoft.EntityFrameworkCore;
using WMS.BLL.Interfaces;
using WMS.DAL;
using WMS.DAL.UnitOfWork;

namespace WMS.BLL.Services;

public class ShippingService : IShippingService
{
    private readonly IUnitOfWork _unitOfWork;

    public ShippingService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<SalesOrder>> GetOrdersReadyForShippingAsync()
    {
        var repo = _unitOfWork.GetRepository<SalesOrder, int>();
        var allOrders = await repo.GetAllWithIncludeAsync(withTracking: false, query => query
            .Include(s => s.Customer)
            .Include(s => s.Warehouse)
            .Include(s => s.SO_Items));

        return allOrders.Where(so => so.Status == SalesOrderStatus.Picked);
    }

    public async Task<string> ShipOrderAsync(int salesOrderId)
    {
        var soRepo = _unitOfWork.GetRepository<SalesOrder, int>();
        var so = await soRepo.GetByIdAsync(
            salesOrderId,
            include: q => q.Include(s => s.SO_Items));

        if (so == null)
            throw new InvalidOperationException("Sales Order not found");

        if (so.Status != SalesOrderStatus.Picked)
            throw new InvalidOperationException("Only Picked Sales Orders can be shipped");

        // Verify all picking tasks are confirmed
        var pickingRepo = _unitOfWork.GetRepository<Picking, int>();
        var itemRepo = _unitOfWork.GetRepository<SO_Item, int>();
        
        var items = await itemRepo.GetAllAsync(withTracking: false);
        var soItemIds = items.Where(i => i.SalesOrderId == salesOrderId).Select(i => i.Id).ToList();

        var allPickings = await pickingRepo.GetAllAsync(withTracking: false);
        var pickingsForSO = allPickings.Where(p => soItemIds.Contains(p.SO_ItemId)).ToList();

        if (pickingsForSO.Any(p => p.Status != PickingStatus.Picked && p.Status != PickingStatus.Cancelled))
        {
            throw new InvalidOperationException("All picking tasks must be confirmed before shipping");
        }

        // Update status
        so.Status = SalesOrderStatus.Shipped;
        soRepo.Update(so);

        // Generate delivery note number
        var deliveryNoteNumber = $"DN-{so.SO_Number}-{DateTime.UtcNow:yyyyMMddHHmmss}";

        await _unitOfWork.CompleteAsync();

        return deliveryNoteNumber;
    }
}

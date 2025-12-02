using Microsoft.EntityFrameworkCore;
using WMS.BLL.Interfaces;
using WMS.DAL;
using WMS.DAL.UnitOfWork;

namespace WMS.BLL.Services;

public class PurchaseOrderService : IPurchaseOrderService
{
    private readonly IUnitOfWork _unitOfWork;

    public PurchaseOrderService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    //public async Task<PurchaseOrder?> GetByIdAsync(int id)
    //{
    //    var repo = _unitOfWork.GetRepository<PurchaseOrder, int>();
    //    var po = await repo.GetByIdAsync(id);
    //    return po;
    //}

    public async Task<PurchaseOrder?> GetByIdAsync(int id)
    {
        var repo = _unitOfWork.GetRepository<PurchaseOrder, int>();

        var po = await repo.GetByIdAsync(
            id,
            include: q => q
                .Include(p => p.POItems)
                    .ThenInclude(i => i.Product)
                .Include(p => p.Vendor)
                .Include(p => p.Warehouse)
        );

        return po;
    }



    public async Task<IEnumerable<PurchaseOrder>> GetAllAsync()
    {
        var repo = _unitOfWork.GetRepository<PurchaseOrder, int>();
        return await repo.GetAllWithIncludeAsync(withTracking: false, query => query
            .Include(p => p.Vendor)
            .Include(p => p.Warehouse));
    }

    public async Task<PurchaseOrder> CreateAsync(PurchaseOrder purchaseOrder)
    {
        // Validation
        if (string.IsNullOrWhiteSpace(purchaseOrder.PO_Number))
            throw new ArgumentException("PO Number is required");
        
        if (purchaseOrder.VendorId <= 0)
            throw new ArgumentException("Vendor is required");
        
        if (purchaseOrder.WarehouseId <= 0)
            throw new ArgumentException("Warehouse is required");

        // Set initial status
        purchaseOrder.Status = PurchaseOrderStatus.Open;
        purchaseOrder.OrderDate = DateTime.UtcNow;

        var repo = _unitOfWork.GetRepository<PurchaseOrder, int>();
        await repo.AddAsync(purchaseOrder);
        await _unitOfWork.CompleteAsync();

        return purchaseOrder;
    }

    public async Task<PurchaseOrder> UpdateAsync(PurchaseOrder purchaseOrder)
    {
        // Validation
        if (purchaseOrder.Id <= 0)
            throw new ArgumentException("Invalid Purchase Order ID");

        var repo = _unitOfWork.GetRepository<PurchaseOrder, int>();
        var existing = await repo.GetByIdAsync(purchaseOrder.Id);
        
        if (existing == null)
            throw new InvalidOperationException("Purchase Order not found");

        // Prevent updates if already closed
        if (existing.Status == PurchaseOrderStatus.Closed)
            throw new InvalidOperationException("Cannot update a closed Purchase Order");

        repo.Update(purchaseOrder);
        await _unitOfWork.CompleteAsync();

        return purchaseOrder;
    }

    public async Task<bool> ApproveAsync(int id)
    {
        var repo = _unitOfWork.GetRepository<PurchaseOrder, int>();
        var po = await repo.GetByIdAsync(id);

        if (po == null)
            throw new InvalidOperationException("Purchase Order not found");

        if (po.Status != PurchaseOrderStatus.Open)
            throw new InvalidOperationException("Only Open Purchase Orders can be approved");

        po.Status = PurchaseOrderStatus.Approved;
        repo.Update(po);
        await _unitOfWork.CompleteAsync();

        return true;
    }

    public async Task<bool> CloseAsync(int id)
    {
        var repo = _unitOfWork.GetRepository<PurchaseOrder, int>();
        var po = await repo.GetByIdAsync(id);

        if (po == null)
            throw new InvalidOperationException("Purchase Order not found");

        if (po.Status != PurchaseOrderStatus.Approved)
            throw new InvalidOperationException("Only Approved Purchase Orders can be closed");

        po.Status = PurchaseOrderStatus.Closed;
        repo.Update(po);
        await _unitOfWork.CompleteAsync();

        return true;
    }

    public async Task<bool> AddItemAsync(int poId, PurchaseOrderItem item)
    {
        // Validation
        if (item.QtyOrdered <= 0)
            throw new ArgumentException("Quantity must be greater than 0");

        if (item.ProductId <= 0)
            throw new ArgumentException("Product is required");

        var poRepo = _unitOfWork.GetRepository<PurchaseOrder, int>();
        var po = await poRepo.GetByIdAsync(poId);

        if (po == null)
            throw new InvalidOperationException("Purchase Order not found");

        if (po.Status == PurchaseOrderStatus.Closed)
            throw new InvalidOperationException("Cannot add items to a closed Purchase Order");

        item.PurchaseOrderId = poId;
        item.LineStatus = "Open";
        item.QtyReceived = 0;

        var itemRepo = _unitOfWork.GetRepository<PurchaseOrderItem, int>();
        await itemRepo.AddAsync(item);
        await _unitOfWork.CompleteAsync();

        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var poRepo = _unitOfWork.GetRepository<PurchaseOrder, int>();
        var po = await poRepo.GetByIdAsync(id);

        if (po == null)
            throw new InvalidOperationException("Purchase Order not found");

        // Check for linked ASNs or Receipts
        var asnRepo = _unitOfWork.GetRepository<AdvancedShippingNotice, int>();
        var asns = await asnRepo.GetAllAsync(WithTracking: false);
        
        if (asns.Any(a => a.PurchaseOrderId == id))
            throw new InvalidOperationException("Cannot delete Purchase Order with linked ASNs");

        poRepo.Delete(po);
        await _unitOfWork.CompleteAsync();

        return true;
    }
}

using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using WMS.BLL.Interfaces;
using WMS.DAL;
using WMS.DAL.UnitOfWork;

namespace WMS.BLL.Services;

public class SalesOrderService : ISalesOrderService
{
    private readonly IUnitOfWork _unitOfWork;

    public SalesOrderService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<SalesOrder?> GetByIdAsync(int id)
    {
        var repo = _unitOfWork.GetRepository<SalesOrder, int>();

        var so = await repo.GetByIdAsync(
            id,
            include: q => q
                .Include(s => s.SO_Items)
                    .ThenInclude(i => i.Product)
                .Include(s => s.Customer)
                .Include(s => s.Warehouse)
        );

        return so;
    }

    public async Task<IEnumerable<SalesOrder>> GetAllAsync()
    {
        var repo = _unitOfWork.GetRepository<SalesOrder, int>();
        return await repo.GetAllWithIncludeAsync(withTracking: false, query => query
            .Include(s => s.Customer)
            .Include(s => s.Warehouse));
    }

    public async Task<(IReadOnlyList<SalesOrder> Items, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        int? customerId = null,
        SalesOrderStatus? status = null,
        DateTime? fromDate = null,
        DateTime? toDate = null)
    {
        var repo = _unitOfWork.GetRepository<SalesOrder, int>();

        // Build filter expression
        System.Linq.Expressions.Expression<Func<SalesOrder, bool>>? filter = null;

        if (customerId.HasValue || status.HasValue || fromDate.HasValue || toDate.HasValue)
        {
            filter = so =>
                (!customerId.HasValue || so.CustomerId == customerId.Value) &&
                (!status.HasValue || so.Status == status.Value) &&
                (!fromDate.HasValue || so.OrderDate >= fromDate.Value) &&
                (!toDate.HasValue || so.OrderDate <= toDate.Value);
        }

        var result = await repo.GetPagedListAsync(
            pageNumber,
            pageSize,
            filter: filter,
            orderBy: q => q.OrderByDescending(s => s.OrderDate),
            includeProperties: "Customer,Warehouse"
        );

        return result;
    }

    public async Task<SalesOrder> CreateAsync(SalesOrder salesOrder)
    {
        // Validation
        if (string.IsNullOrWhiteSpace(salesOrder.SO_Number))
            throw new ArgumentException("SO Number is required");

        if (salesOrder.CustomerId <= 0)
            throw new ArgumentException("Customer is required");

        if (salesOrder.WarehouseId <= 0)
            throw new ArgumentException("Warehouse is required");

        // Verify customer exists
        var customerRepo = _unitOfWork.GetRepository<Customer, int>();
        var customer = await customerRepo.GetByIdAsync(salesOrder.CustomerId);
        if (customer == null)
            throw new ArgumentException("Customer not found");

        // Set initial status
        salesOrder.Status = SalesOrderStatus.Draft;
        salesOrder.OrderDate = DateTime.UtcNow;

        var repo = _unitOfWork.GetRepository<SalesOrder, int>();
        await repo.AddAsync(salesOrder);
        await _unitOfWork.CompleteAsync();

        return salesOrder;
    }

    public async Task<SalesOrder> UpdateAsync(SalesOrder salesOrder)
    {
        // Validation
        if (salesOrder.Id <= 0)
            throw new ArgumentException("Invalid Sales Order ID");

        var repo = _unitOfWork.GetRepository<SalesOrder, int>();
        var existing = await repo.GetByIdAsync(salesOrder.Id);

        if (existing == null)
            throw new InvalidOperationException("Sales Order not found");

        // Prevent updates if not Draft
        if (existing.Status != SalesOrderStatus.Draft)
            throw new InvalidOperationException("Cannot update a Sales Order that is not in Draft status");

        repo.Update(salesOrder);
        await _unitOfWork.CompleteAsync();

        return salesOrder;
    }

    public async Task<bool> SubmitAsync(int id)
    {
        var repo = _unitOfWork.GetRepository<SalesOrder, int>();
        var so = await repo.GetByIdAsync(
            id,
            include: q => q.Include(s => s.SO_Items)
        );

        if (so == null)
            throw new InvalidOperationException("Sales Order not found");

        if (so.Status != SalesOrderStatus.Draft)
            throw new InvalidOperationException("Only Draft Sales Orders can be submitted");

        // Validation: Must have at least one item
        if (so.SO_Items == null || !so.SO_Items.Any())
            throw new InvalidOperationException("Cannot submit an empty Sales Order. Please add at least one item.");

        // Verify customer exists
        var customerRepo = _unitOfWork.GetRepository<Customer, int>();
        var customer = await customerRepo.GetByIdAsync(so.CustomerId);
        if (customer == null)
            throw new InvalidOperationException("Customer not found");

        so.Status = SalesOrderStatus.Submitted;
        repo.Update(so);
        await _unitOfWork.CompleteAsync();

        return true;
    }

    public async Task<bool> AddItemAsync(int soId, SO_Item item)
    {
        // Validation
        if (item.QtyOrdered <= 0)
            throw new ArgumentException("Quantity must be greater than 0");

        if (item.ProductId <= 0)
            throw new ArgumentException("Product is required");

        if (item.UnitPrice <= 0)
            throw new ArgumentException("Unit Price must be greater than 0");

        var soRepo = _unitOfWork.GetRepository<SalesOrder, int>();
        var so = await soRepo.GetByIdAsync(soId);

        if (so == null)
            throw new InvalidOperationException("Sales Order not found");

        if (so.Status != SalesOrderStatus.Draft)
            throw new InvalidOperationException("Cannot add items to a Sales Order that is not in Draft status");

        // Verify product exists
        var productRepo = _unitOfWork.GetRepository<Product, int>();
        var product = await productRepo.GetByIdAsync(item.ProductId);
        if (product == null)
            throw new ArgumentException("Product not found");

        item.SalesOrderId = soId;
        item.QtyPicked = 0;

        var itemRepo = _unitOfWork.GetRepository<SO_Item, int>();
        await itemRepo.AddAsync(item);
        await _unitOfWork.CompleteAsync();

        return true;
    }

    public async Task<bool> RemoveItemAsync(int itemId)
    {
        var itemRepo = _unitOfWork.GetRepository<SO_Item, int>();
        var item = await itemRepo.GetByIdAsync(itemId);

        if (item == null)
            throw new InvalidOperationException("Sales Order Item not found");

        // Get the Sales Order to check status
        var soRepo = _unitOfWork.GetRepository<SalesOrder, int>();
        var so = await soRepo.GetByIdAsync(item.SalesOrderId);

        if (so == null)
            throw new InvalidOperationException("Sales Order not found");

        if (so.Status != SalesOrderStatus.Draft)
            throw new InvalidOperationException("Cannot remove items from a Sales Order that is not in Draft status");

        itemRepo.Delete(item);
        await _unitOfWork.CompleteAsync();

        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var soRepo = _unitOfWork.GetRepository<SalesOrder, int>();
        var so = await soRepo.GetByIdAsync(id);

        if (so == null)
            throw new InvalidOperationException("Sales Order not found");

        // Check for linked picking tasks
        var pickingRepo = _unitOfWork.GetRepository<Picking, int>();
        var pickings = await pickingRepo.GetAllAsync(false);

        // Get all SO_Items for this SO
        var itemRepo = _unitOfWork.GetRepository<SO_Item, int>();
        var items = await itemRepo.GetAllAsync(false);
        var soItemIds = items.Where(i => i.SalesOrderId == id).Select(i => i.Id).ToList();

        if (pickings.Any(p => soItemIds.Contains(p.SO_ItemId)))
            throw new InvalidOperationException("Cannot delete Sales Order with linked Picking tasks");

        soRepo.Delete(so);
        await _unitOfWork.CompleteAsync();

        return true;
    }
}

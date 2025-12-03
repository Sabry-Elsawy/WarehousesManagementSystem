using WMS.DAL;

namespace WMS.BLL.Interfaces;

public interface ISalesOrderService
{
    Task<SalesOrder?> GetByIdAsync(int id);
    Task<IEnumerable<SalesOrder>> GetAllAsync();
    Task<(IReadOnlyList<SalesOrder> Items, int TotalCount)> GetPagedAsync(
        int pageNumber, 
        int pageSize, 
        int? customerId = null, 
        SalesOrderStatus? status = null, 
        DateTime? fromDate = null, 
        DateTime? toDate = null);
    Task<SalesOrder> CreateAsync(SalesOrder salesOrder);
    Task<SalesOrder> UpdateAsync(SalesOrder salesOrder);
    Task<bool> SubmitAsync(int id);
    Task<bool> AddItemAsync(int soId, SO_Item item);
    Task<bool> RemoveItemAsync(int itemId);
    Task<bool> DeleteAsync(int id);
}

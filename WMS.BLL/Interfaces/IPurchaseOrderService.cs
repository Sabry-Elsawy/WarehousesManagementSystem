using WMS.DAL;

namespace WMS.BLL.Interfaces;

public interface IPurchaseOrderService
{
    Task<PurchaseOrder?> GetByIdAsync(int id);
    Task<IEnumerable<PurchaseOrder>> GetAllAsync();
    Task<PurchaseOrder> CreateAsync(PurchaseOrder purchaseOrder);
    Task<PurchaseOrder> UpdateAsync(PurchaseOrder purchaseOrder);
    Task<bool> ApproveAsync(int id);
    Task<bool> CloseAsync(int id);
    Task<bool> AddItemAsync(int poId, PurchaseOrderItem item);
    Task<bool> DeleteAsync(int id);
}

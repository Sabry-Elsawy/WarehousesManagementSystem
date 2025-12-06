using WMS.DAL;

namespace WMS.BLL.Interfaces;

public interface IPickingService
{
    Task<IEnumerable<Picking>> GetAllAsync();
    Task<IEnumerable<Picking>> GetBySalesOrderIdAsync(int soId);
    Task<Picking?> GetByIdAsync(int id);
    Task<bool> AllocatePickingTasksAsync(int salesOrderId);
    Task<bool> StartPickingAsync(int pickingId, string performedBy);
    Task<bool> ConfirmPickingAsync(int pickingId, int quantityPicked);
    Task<bool> CancelPickingAsync(int pickingId);
}

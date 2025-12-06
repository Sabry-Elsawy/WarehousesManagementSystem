using WMS.DAL;

namespace WMS.BLL.Interfaces;

public interface ITransferOrderService
{
    Task<IReadOnlyList<TransferOrder>> GetAllAsync();
    Task<TransferOrder?> GetByIdAsync(int id);
    Task CreateAsync(TransferOrder transferOrder);
    Task ApproveAsync(int id);
    Task IssueAsync(int id);
    Task ReceiveAsync(int id);
    Task AddItemAsync(int transferOrderId, int productId, int quantity);
    Task CancelAsync(int id);
}

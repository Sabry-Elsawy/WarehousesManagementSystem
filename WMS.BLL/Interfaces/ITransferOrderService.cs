using WMS.DAL;

namespace WMS.BLL.Interfaces;

public interface ITransferOrderService
{
    Task<IReadOnlyList<TransferOrder>> GetAllAsync();
    Task<TransferOrder?> GetByIdAsync(int id);
    Task CreateAsync(TransferOrder transferOrder);
    Task ApproveAsync(int id);
}

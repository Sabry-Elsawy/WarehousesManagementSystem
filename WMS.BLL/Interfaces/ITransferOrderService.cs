using WMS.DAL;

namespace WMS.BLL.Interfaces
{
    public interface ITransferOrderService
    {
        Task<(IEnumerable<TransferOrder> Items, int TotalCount)> GetPagedListAsync(
            int pageNumber, 
            int pageSize, 
            string searchTerm = null, 
            string sortBy = null, 
            string sortOrder = "asc");

        Task<TransferOrder?> GetByIdAsync(int id);
        Task<TransferOrder> CreateAsync(TransferOrder transferOrder);
        Task UpdateStatusAsync(int id, TransferOrderStatus status);
    }
}

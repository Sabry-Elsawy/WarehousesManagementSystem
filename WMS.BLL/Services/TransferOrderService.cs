using Microsoft.EntityFrameworkCore;
using WMS.BLL.Interfaces;
using WMS.DAL;
using WMS.DAL.UnitOfWork;

namespace WMS.BLL.Services
{
    public class TransferOrderService : ITransferOrderService
    {
        private readonly IUnitOfWork _unitOfWork;

        public TransferOrderService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<(IEnumerable<TransferOrder> Items, int TotalCount)> GetPagedListAsync(
            int pageNumber,
            int pageSize,
            string searchTerm = null,
            string sortBy = null,
            string sortOrder = "asc")
        {
            var repository = _unitOfWork.GetRepository<TransferOrder, int>();

            // Includes for related data
            var includeProperties = "SourceWarehouse,DestinationWarehouse,TransferOrderItems,TransferOrderItems.Product";

            // Filter
            System.Linq.Expressions.Expression<Func<TransferOrder, bool>> filter = null;
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                // Simple search by ID or Status (since we don't have a name/description)
                // Assuming searchTerm might be the ID
                if (int.TryParse(searchTerm, out int searchId))
                {
                    filter = x => x.Id == searchId;
                }
            }

            return await repository.GetPagedListAsync(
                pageNumber,
                pageSize,
                filter,
                orderBy: null, // We'll handle sorting if needed, or rely on repository default
                includeProperties: includeProperties
            );
        }

        public async Task<TransferOrder?> GetByIdAsync(int id)
        {
            var repository = _unitOfWork.GetRepository<TransferOrder, int>();
            
            return await repository.GetByIdAsync(id, query => query
                .Include(x => x.SourceWarehouse)
                .Include(x => x.DestinationWarehouse)
                .Include(x => x.TransferOrderItems)
                    .ThenInclude(i => i.Product));
        }

        public async Task<TransferOrder> CreateAsync(TransferOrder transferOrder)
        {
            // Basic validation
            if (transferOrder.SourceWarehouseId == transferOrder.DestinationWarehouseId)
            {
                throw new InvalidOperationException("Source and Destination warehouses cannot be the same.");
            }

            if (transferOrder.TransferOrderItems == null || !transferOrder.TransferOrderItems.Any())
            {
                throw new InvalidOperationException("Transfer Order must have at least one item.");
            }

            transferOrder.Status = TransferOrderStatus.Pending; // Default status
            transferOrder.CreatedOn = DateTime.UtcNow;

            var repository = _unitOfWork.GetRepository<TransferOrder, int>();
            await repository.AddAsync(transferOrder);
            await _unitOfWork.CompleteAsync();

            return transferOrder;
        }

        public async Task UpdateStatusAsync(int id, TransferOrderStatus status)
        {
            var repository = _unitOfWork.GetRepository<TransferOrder, int>();
            var order = await repository.GetByIdAsync(id);
            if (order == null)
            {
                throw new KeyNotFoundException($"Transfer Order with ID {id} not found.");
            }

            order.Status = status;
            repository.Update(order);
            await _unitOfWork.CompleteAsync();
        }
    }
}

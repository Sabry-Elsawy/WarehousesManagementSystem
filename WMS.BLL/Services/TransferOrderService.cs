using Microsoft.EntityFrameworkCore;
using WMS.BLL.Interfaces;
using WMS.DAL;
using WMS.DAL.UnitOfWork;

namespace WMS.BLL.Services;

public class TransferOrderService : ITransferOrderService
{
    private readonly IUnitOfWork _unitOfWork;

    public TransferOrderService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyList<TransferOrder>> GetAllAsync()
    {
        var repository = _unitOfWork.GetRepository<TransferOrder, int>();
        var orders = await repository.GetAllWithIncludeAsync(false, q => q
            .Include(t => t.SourceWarehouse)
            .Include(t => t.DestinationWarehouse)
            .Include(t => t.TransferOrderItems)
            .ThenInclude(i => i.Product));
        return orders.ToList();
    }

    public async Task<TransferOrder?> GetByIdAsync(int id)
    {
        var repository = _unitOfWork.GetRepository<TransferOrder, int>();
        return await repository.GetByIdAsync(id, q => q
            .Include(t => t.SourceWarehouse)
            .Include(t => t.DestinationWarehouse)
            .Include(t => t.TransferOrderItems)
            .ThenInclude(i => i.Product));
    }

    public async Task CreateAsync(TransferOrder transferOrder)
    {
        var repository = _unitOfWork.GetRepository<TransferOrder, int>();
        transferOrder.Status = TransferOrderStatus.Pending;
        await repository.AddAsync(transferOrder);
        await _unitOfWork.CompleteAsync();
    }

    public async Task ApproveAsync(int id)
    {
        var repository = _unitOfWork.GetRepository<TransferOrder, int>();
        var order = await repository.GetByIdAsync(id);
        if (order != null && order.Status == TransferOrderStatus.Pending)
        {
            order.Status = TransferOrderStatus.Approved;
            repository.Update(order);
            await _unitOfWork.CompleteAsync();
        }
    }
}

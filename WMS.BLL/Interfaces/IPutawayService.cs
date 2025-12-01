

using WMS.BLL.DTOs;
using WMS.DAL;
using WMS.DAL.Entities;


namespace WMS.BLL.Interfaces;

public interface IPutawayService
{
    Task<IEnumerable<Putaway>> GetAllAsync();
    Task<Putaway?> GetByIdAsync(int id);
    Task<IEnumerable<BinDTO>> GetAvailableBinsAsync(int putawayId);
    Task<bool> AssignAsync(int putawayId, int binId, int qty);
    Task<bool> CompleteAsync(int putawayId);
}

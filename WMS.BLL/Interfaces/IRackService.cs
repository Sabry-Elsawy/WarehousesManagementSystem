using WMS.DAL;

namespace WMS.BLL.Interfaces
{
    public interface IRackService
    {
        Task<Rack> CreateRackAsync(Rack rack);
        Task<Rack?> GetRackByIdAsync(int id);
        Task<IReadOnlyList<Rack>> GetAllRacksAsync();
        Task<IReadOnlyList<Rack>> GetRacksByAisleIdAsync(int aisleId);
        Task<Rack> UpdateRackAsync(Rack rack);
        Task DeleteRackAsync(int id);
    }
}

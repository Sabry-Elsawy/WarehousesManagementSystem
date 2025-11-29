using WMS.DAL;

namespace WMS.BLL.Interfaces
{
    public interface IAisleService
    {
        Task<Aisle> CreateAisleAsync(Aisle aisle);
        Task<Aisle?> GetAisleByIdAsync(int id);
        Task<IReadOnlyList<Aisle>> GetAllAislesAsync();
        Task<IReadOnlyList<Aisle>> GetAislesByZoneIdAsync(int zoneId);
        Task<Aisle> UpdateAisleAsync(Aisle aisle);
        Task DeleteAisleAsync(int id);
    }
}

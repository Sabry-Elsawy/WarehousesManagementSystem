using WMS.DAL;

namespace WMS.BLL.Interfaces
{
    public interface IZoneService
    {
        Task<Zone> CreateZoneAsync(Zone zone);
        Task<Zone?> GetZoneByIdAsync(int id);
        Task<IReadOnlyList<Zone>> GetAllZonesAsync();
        Task<IReadOnlyList<Zone>> GetZonesByWarehouseIdAsync(int warehouseId);
        Task<Zone> UpdateZoneAsync(Zone zone);
        Task DeleteZoneAsync(int id);
    }
}

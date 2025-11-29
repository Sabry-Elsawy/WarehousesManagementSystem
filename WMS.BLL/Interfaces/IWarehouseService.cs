using WMS.DAL;

namespace WMS.BLL.Interfaces
{
    public interface IWarehouseService
    {
        Task<Warehouse> CreateWarehouseAsync(Warehouse warehouse);
        Task<Warehouse?> GetWarehouseByIdAsync(int id);
        Task<IReadOnlyList<Warehouse>> GetAllWarehousesAsync();
        Task<Warehouse> UpdateWarehouseAsync(Warehouse warehouse);
        Task DeleteWarehouseAsync(int id);
    }
}

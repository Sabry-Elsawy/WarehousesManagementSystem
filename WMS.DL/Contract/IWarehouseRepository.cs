using WMS.DAL.Entities._Common;

namespace WMS.DAL.Contract
{
    public interface IWarehouseRepository : IGenericRepository<Warehouse, int>
    {
        Task<Warehouse?> GetWarehouseWithDetailsAsync(int id);
        Task<IReadOnlyList<Warehouse>> GetAllWithDetailsAsync();
        Task<bool> HasDependenciesAsync(int id);
    }
}

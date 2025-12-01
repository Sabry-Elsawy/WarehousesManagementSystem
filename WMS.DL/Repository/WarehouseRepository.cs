using Microsoft.EntityFrameworkCore;
using WMS.DAL.Contract;
using WMS.DAL.Data;

namespace WMS.DAL.Repository
{
    public class WarehouseRepository : GenericRepository<Warehouse, int>, IWarehouseRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public WarehouseRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Warehouse?> GetWarehouseWithDetailsAsync(int id)
        {
            return await _dbContext.Warehouses
                .Include(w => w.Zones)
                .ThenInclude(z => z.Aisles)
                .FirstOrDefaultAsync(w => w.Id == id);
        }

        public async Task<IReadOnlyList<Warehouse>> GetAllWithDetailsAsync()
        {
            return await _dbContext.Warehouses
                .Include(w => w.Zones)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<bool> HasDependenciesAsync(int id)
        {
            return await _dbContext.Warehouses
                .AnyAsync(w => w.Id == id && (w.Zones.Any() || w.PurchaseOrders.Any() || w.SalesOrders.Any()));
        }
    }
}

using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WMS.DAL.Contract;
using WMS.DAL.Data;
using WMS.DAL.Entities;

namespace WMS.DAL.Repository
{
    public class BinRepository : GenericRepository<Bin, int>, IBinRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public BinRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Bin?> GetBinWithDetailsAsync(int id)
        {
            return await _dbContext.Bins
                .Include(b => b.Rack)
                .Include(b => b.BinType)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<IReadOnlyList<Bin>> GetAllBinsWithDetailsAsync()
        {
            return await _dbContext.Bins
                .Include(b => b.Rack)
                .Include(b => b.BinType)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}

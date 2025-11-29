using System.Collections.Generic;
using System.Threading.Tasks;
using WMS.DAL.Entities;

namespace WMS.DAL.Contract
{
    public interface IBinRepository : IGenericRepository<Bin, int>
    {
        Task<Bin?> GetBinWithDetailsAsync(int id);
        Task<IReadOnlyList<Bin>> GetAllBinsWithDetailsAsync();
    }
}

using System.Collections.Generic;
using System.Threading.Tasks;
using WMS.DAL.Entities;

namespace WMS.BLL.Interfaces
{
    public interface IBinService
    {
        Task<Bin> CreateBinAsync(Bin bin);
        Task<Bin?> GetBinByIdAsync(int id);
        Task<IReadOnlyList<Bin>> GetAllBinsAsync();
        Task<Bin> UpdateBinAsync(Bin bin);
        Task DeleteBinAsync(int id);
    }
}
using System.Collections.Generic;
using System.Threading.Tasks;
using WMS.DAL.Entities;

namespace WMS.BLL.Interfaces
{
    public interface IBinTypeService
    {
        Task<BinType> CreateBinTypeAsync(BinType binType);
        Task<BinType?> GetBinTypeByIdAsync(int id);
        Task<IReadOnlyList<BinType>> GetAllBinTypesAsync();
        Task<BinType> UpdateBinTypeAsync(BinType binType);
        Task DeleteBinTypeAsync(int id);
    }
}
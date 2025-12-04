

using WMS.BLL.DTOs;

namespace WMS.BLL.Interfaces;
public interface IInventoryService
{
    Task<IEnumerable<InventoryListDto>> GetAllAsync(string? search = null);
    Task<bool> AddInventoryAsync(AddInventoryDto dto);
    Task<bool> AdjustInventoryAsync(AdjustInventoryDto dto);
}

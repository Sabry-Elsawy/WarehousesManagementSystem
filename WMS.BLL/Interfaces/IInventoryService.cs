

using WMS.BLL.DTOs;

namespace WMS.BLL.Interfaces;
public interface IInventoryService
{
    Task<IEnumerable<InventoryListDto>> GetAllAsync(string? search = null);
    Task<InventoryDetailsDto?> GetByIdAsync(int id);
    Task<IEnumerable<InventoryListDto>> GetByProductIdAsync(int productId);
    Task<IEnumerable<InventoryListDto>> GetByBinIdAsync(int binId);
    Task<(IEnumerable<InventoryListDto> Items, int TotalCount)> GetPagedAsync(
        int pageNumber, int pageSize, 
        string? search, int? productId, int? binId, string? status);
    Task<bool> AddInventoryAsync(AddInventoryDto dto);
    Task<bool> AdjustInventoryAsync(AdjustInventoryDto dto);
    Task<bool> TransferInventoryAsync(TransferInventoryDto dto);
    Task<Dictionary<string, int>> GetStockSummaryAsync();
    Task<IEnumerable<InventoryTransactionDto>> GetTransactionsAsync(int? productId = null, int? binId = null);
}

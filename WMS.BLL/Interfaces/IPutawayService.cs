

using WMS.BLL.DTOs;
using WMS.DAL;
using WMS.DAL.Entities;


namespace WMS.BLL.Interfaces;

public interface IPutawayService
{
    // New comprehensive methods
    Task<PutawayDto> CreatePutawayForReceiptItemAsync(int receiptItemId, int qty, string createdBy);
    Task<AutoAssignResultDto> AutoAssignBinsAsync(int putawayId);
    Task AssignBinsManualAsync(int putawayId, IEnumerable<PutawayBinDto> assignments, string assignedBy);
    Task ExecutePutawayAsync(int putawayId, string performedBy);
    Task ClosePutawayAsync(int putawayId, string closedBy);
    Task<(IEnumerable<PutawayDto> Items, int TotalCount)> GetPagedPutawaysAsync(int pageNumber, int pageSize, string? searchTerm, string? statusFilter);
    Task<PutawayDto?> GetPutawayDetailsAsync(int id);
    
    // Legacy methods (kept for backwards compatibility, can be removed later)
    Task<IEnumerable<Putaway>> GetAllAsync();
    Task<Putaway?> GetByIdAsync(int id);
    Task<IEnumerable<BinDTO>> GetAvailableBinsAsync(int putawayId);
    Task<bool> AssignAsync(int putawayId, int binId, int qty);
    Task<bool> CompleteAsync(int putawayId);
}

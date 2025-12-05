using WMS.BLL.DTOs;

namespace WMS.BLL.Interfaces;

public interface ICycleCountService
{
    Task<int> CreateSessionAsync(CreateCycleCountDto dto, string createdBy);
    Task<CycleCountDto?> GetSessionAsync(int id);
    Task<IEnumerable<CycleCountDto>> GetAllSessionsAsync();
    Task<IEnumerable<CycleCountItemDto>> GetSessionItemsAsync(int sessionId);
    Task<bool> UpdateCountAsync(UpdateCountDto dto);
    Task<bool> FinalizeSessionAsync(int sessionId, string performedBy);
}

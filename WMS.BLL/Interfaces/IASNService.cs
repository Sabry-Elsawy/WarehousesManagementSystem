using WMS.DAL;

namespace WMS.BLL.Interfaces;

public interface IASNService
{
    Task<AdvancedShippingNotice?> GetByIdAsync(int id);
    Task<IEnumerable<AdvancedShippingNotice>> GetAllAsync();
    Task<AdvancedShippingNotice> CreateFromPOAsync(int purchaseOrderId, AdvancedShippingNotice asn);
    Task<AdvancedShippingNotice> UpdateAsync(AdvancedShippingNotice asn);
    Task<bool> MarkReceivedAsync(int asnId);
    Task<bool> CloseAsync(int asnId);
    Task<bool> AddItemAsync(int asnId, AdvancedShippingNoticeItem item);
}

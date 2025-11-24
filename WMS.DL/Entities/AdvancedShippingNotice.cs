using WMS.DAL.Entities._Common;

namespace WMS.DAL;

public class AdvancedShippingNotice : BaseAuditableEntity<int>
{
    public DateTime ExpectedArrivalDate { get; set; }
    public string SKU { get; set; } = null!;
    public AdvancedShippingNoticeStatus Status { get; set; }


    // Navigation Properties
    public int PurchaseOrderId { get; set; }
    public PurchaseOrder PurchaseOrder { get; set; } = null!;

    public IReadOnlyCollection<AdvancedShippingNoticeItem> ASNItems { get; set; } = new List<AdvancedShippingNoticeItem>();
}
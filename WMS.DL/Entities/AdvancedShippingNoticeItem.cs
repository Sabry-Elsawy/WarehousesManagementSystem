using WMS.DAL.Entities._Common;

namespace WMS.DAL;

public class AdvancedShippingNoticeItem : BaseAuditableEntity<int>
{
    public int QtyShipped { get; set; }
    public int? LinkedPOItemId { get; set; }
    public string SKU { get; set; } = null!;

    // Navigation Properties

    public int AdvancedShippingNoticeId { get; set; }
    public AdvancedShippingNotice AdvancedShippingNotice { get; set; } = null!;

    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;


}
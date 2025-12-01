using WMS.DAL.Entities._Common;

namespace WMS.DAL;

public class ReceiptItem : BaseAuditableEntity<int>
{
    public int QtyExpected { get; set; }
    public int QtyReceived { get; set; }
    public DiscrepancyType DiscrepancyType { get; set; }
    public string? Notes { get; set; }
    public int ASNItemId { get; set; }
    public AdvancedShippingNoticeItem ASNItem { get; set; } = null!;
    public string SKU { get; set; } = null!;



    // Navigation Properties
    public int ReceiptId { get; set; }
    public Receipt Receipt { get; set; } = null!;
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public IReadOnlyCollection<Putaway> Putaways { get; set; } = new List<Putaway>();


}
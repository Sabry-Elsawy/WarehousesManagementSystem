namespace WMS.DAL;

public class AdvancedShippingNoticeItem
{
    public int Id { get; set; }
    public int Qty { get; set; }
    public string SKU { get; set; } = null!;

    // Navigation Properties

    public int AdvancedShippingNoticeId { get; set; }
    public AdvancedShippingNotice AdvancedShippingNotice { get; set; } = null!;

    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;


}
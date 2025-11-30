using WMS.DAL.Entities._Common;

namespace WMS.DAL;

public class PurchaseOrderItem : BaseAuditableEntity<int>
{
    public int QtyOrdered { get; set; }
    public decimal UnitPrice { get; set; }
    public int QtyReceived { get; set; }
    public string LineStatus { get; set; } = "Open";
    public string SKU { get; set; } = null!;


    // Navigation Properties
    public int PurchaseOrderId { get; set; }
    public PurchaseOrder PurchaseOrder { get; set; } = null!;

    public Product Product { get; set; } = null!;
    public int ProductId { get; set; }

}
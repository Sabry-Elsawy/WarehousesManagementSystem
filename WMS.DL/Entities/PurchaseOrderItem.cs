using WMS.DAL.Entities._Common;

namespace WMS.DAL;

public class PurchaseOrderItem : BaseAuditableEntity<int>
{
    public int Qty { get; set; }
    public string SKU { get; set; } = null!;


    // Navigation Properties
    public int PurchaseOrderId { get; set; }
    public PurchaseOrder PurchaseOrder { get; set; } = null!;

    public Product Product { get; set; } = null!;
    public int ProductId { get; set; }

}
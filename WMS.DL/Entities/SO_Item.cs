using WMS.DAL.Entities._Common;

namespace WMS.DAL;

public class SO_Item : BaseAuditableEntity<int>
{
    public int SalesOrderId { get; set; }
    public int ProductId { get; set; }
    public int QtyOrdered { get; set; }
    public int QtyPicked { get; set; }
    public decimal UnitPrice { get; set; }
    
    // Backorder Tracking
    public bool IsBackorder { get; set; } = false;
    public int? OriginalSOItemId { get; set; }

    // Navigation properties
    public SalesOrder SalesOrder { get; set; } = null!;
    public Product Product { get; set; } = null!;
}

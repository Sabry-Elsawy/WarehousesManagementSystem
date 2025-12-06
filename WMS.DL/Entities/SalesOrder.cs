using WMS.DAL.Entities._Common;

namespace WMS.DAL;

public class SalesOrder : BaseAuditableEntity<int>
{
    public string SO_Number { get; set; } = null!;
    public DateTime OrderDate { get; set; }
    public SalesOrderStatus Status { get; set; }
    
    // Shipping Information
    public DateTime? ShippedDate { get; set; }
    public string? Carrier { get; set; }
    public string? TrackingNumber { get; set; }
    
    // Cancellation Information
    public DateTime? CancelledOn { get; set; }
    public string? CancellationReason { get; set; }

    // Foreign Keys
    public int CustomerId { get; set; }
    public int WarehouseId { get; set; }

    // Navigation Properties
    public Customer Customer { get; set; } = null!;
    public Warehouse Warehouse { get; set; } = null!;
    public ICollection<SO_Item> SO_Items { get; set; } = new List<SO_Item>();
}

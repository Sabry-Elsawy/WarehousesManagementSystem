using WMS.DAL.Entities;
using WMS.DAL.Entities._Common;

namespace WMS.DAL;

public class Picking : BaseAuditableEntity<int>
{
    public int QuantityToPick { get; set; }
    public int QuantityPicked { get; set; }
    public PickingStatus Status { get; set; }
    
    // Picking Tracking
    public DateTime? StartedOn { get; set; }
    public string? PickedBy { get; set; }
    
    // Inventory Reservation
    public int? ReservedInventoryId { get; set; }
    
    // Shortage/Backorder Tracking
    public int? ShortageQuantity { get; set; }
    public string? ShortageReason { get; set; }
    public int? OriginalPickingId { get; set; }

    // Foreign Keys
    public int SO_ItemId { get; set; }
    public int ProductId { get; set; }
    public int BinId { get; set; }

    // Navigation Properties
    public SO_Item SO_Item { get; set; } = null!;
    public Product Product { get; set; } = null!;
    public Bin Bin { get; set; } = null!;
}

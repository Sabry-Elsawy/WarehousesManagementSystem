using WMS.DAL.Entities;
using WMS.DAL.Entities._Common;

namespace WMS.DAL;

public class Picking : BaseAuditableEntity<int>
{
    public int QuantityToPick { get; set; }
    public int QuantityPicked { get; set; }
    public PickingStatus Status { get; set; }

    // Foreign Keys
    public int SO_ItemId { get; set; }
    public int ProductId { get; set; }
    public int BinId { get; set; }

    // Navigation Properties
    public SO_Item SO_Item { get; set; } = null!;
    public Product Product { get; set; } = null!;
    public Bin Bin { get; set; } = null!;
}


using WMS.DAL.Entities._Common;

namespace WMS.DAL;

public class TransferOrder : BaseAuditableEntity<int>
{
    public TransferOrderStatus Status { get; set; }



    // Navigation properties
    public IReadOnlyCollection<TransferOrderItem> TransferOrderItems { get; set; } = new List<TransferOrderItem>();

    public int SourceWarehouseId { get; set; }
    public Warehouse SourceWarehouse { get; set; } = null!;

    public int DestinationWarehouseId { get; set; }
    public Warehouse DestinationWarehouse { get; set; } = null!;

}

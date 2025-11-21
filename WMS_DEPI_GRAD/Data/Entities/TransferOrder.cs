namespace WMS_DEPI_GRAD;

public class TransferOrder
{
    public int Id { get; set; }
    public TransferOrderStatus Status { get; set; }



    // Navigation properties
    public IReadOnlyCollection<TransferOrderItem> TransferOrderItems { get; set; } = new List<TransferOrderItem>();

    public int SourceWarehouseId { get; set; }
    public Warehouse SourceWarehouse { get; set; } = null!;

    public int DestinationWarehouseId { get; set; }
    public Warehouse DestinationWarehouse { get; set; } = null!;

}

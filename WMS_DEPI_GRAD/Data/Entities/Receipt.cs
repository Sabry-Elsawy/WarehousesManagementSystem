namespace WMS_DEPI_GRAD;

public class Receipt
{
    public int Id { get; set; }
    public DateTime RecievedDate { get; set; }





    //Navigation Properties
    public int AdvancedShippingNoticeId { get; set; }
    public AdvancedShippingNotice AdvancedShippingNotice { get; set; } = null!;

    public IReadOnlyCollection<ReceiptItem> ReceiptItems { get; set; } = new List<ReceiptItem>();

    public Warehouse Warehouse { get; set; } = null!;
    public int WarehouseId { get; set; }

}

using WMS.DAL.Entities._Common;

namespace WMS.DAL;

public class Receipt : BaseAuditableEntity<int>
{
    public string ReceiptNumber { get; set; } = null!;
    public DateTime ReceivedDate { get; set; }
    public ReceiptStatus Status { get; set; }





    //Navigation Properties
    public int AdvancedShippingNoticeId { get; set; }
    public AdvancedShippingNotice AdvancedShippingNotice { get; set; } = null!;

    public IReadOnlyCollection<ReceiptItem> ReceiptItems { get; set; } = new List<ReceiptItem>();

    public Warehouse Warehouse { get; set; } = null!;
    public int WarehouseId { get; set; }

}

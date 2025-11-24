using WMS.DAL.Entities._Common;

namespace WMS.DAL;

public class TransferOrderItem : BaseAuditableEntity<int>
{
    public int Qty { get; set; }



    //Navigation Properties

    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public TransferOrder TransferOrder { get; set; } = null!;   
    public int TransferOrderId { get; set; }
}
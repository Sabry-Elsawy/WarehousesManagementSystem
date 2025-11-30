
using WMS.DAL.Entities._Common;

namespace WMS.DAL;

public class PurchaseOrder : BaseAuditableEntity<int>
{
    public string PO_Number { get; set; } = null!;
    public DateTime OrderDate { get; set; }
    public DateTime ExpectedArrivalDate { get; set; } 

    public PurchaseOrderStatus Status { get; set; }




    // Navigation Properties

    public IReadOnlyCollection<AdvancedShippingNotice> ASNs { get; set; } = new List<AdvancedShippingNotice> ();

    public IReadOnlyCollection<PurchaseOrderItem> POItems { get; set; } = new List<PurchaseOrderItem>();

    public Warehouse Warehouse { get; set; } = null!;
    public int WarehouseId { get; set; }
    public int VendorId { get; set; }
    public Vendor Vendor { get; set; } = null!;


}

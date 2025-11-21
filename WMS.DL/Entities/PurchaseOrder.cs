
namespace WMS.DAL;

public class PurchaseOrder
{
    public int Id { get; set; } 
    public DateTime OrderDate { get; set; } 

    public PurchaseOrderStatus Status { get; set; }




    // Navigation Properties

    public IReadOnlyCollection<AdvancedShippingNotice> ASNs { get; set; } = new List<AdvancedShippingNotice> ();

    public IReadOnlyCollection<PurchaseOrderItem> POItems { get; set; } = new List<PurchaseOrderItem>();

    public Warehouse Warehouse { get; set; } = null!;
    public int WarehouseId { get; set; }
    public int VendorId { get; set; }
    public Vendor Vendor { get; set; } = null!;


}

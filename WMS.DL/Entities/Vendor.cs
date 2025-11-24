using WMS.DAL.Entities._Common;

namespace WMS.DAL;

public class Vendor : BaseAuditableEntity<int>
{
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string ContactEmail { get; set; } = string.Empty;

    // Navigation property
    public ICollection<PurchaseOrder> PurchaseOrders { get; set; } = new List<PurchaseOrder>();
}

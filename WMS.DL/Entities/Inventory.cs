using WMS.DAL.Entities;
using WMS.DAL.Entities._Common;

namespace WMS.DAL;

public class Inventory : BaseAuditableEntity<int>
{
    public string Status { get; set; }
    public int Quantity { get; set; }
    public int ReservedQuantity { get; set; } = 0;  // NEW: Track reserved qty
    public string BatchNumber { get; set; }
    public string ExpiryDate { get; set; }

    public int ProductId { get; set; }
    public Product Product { get; set; }

    public int BinId { get; set; }
    public Bin Bin { get; set; }
}

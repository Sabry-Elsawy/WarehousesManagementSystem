using WMS.DAL.Entities._Common;

namespace WMS.DAL;

public class SalesOrder : BaseAuditableEntity<int>
{
    public DateTime OrderDate { get; set; }
    public string CustomerRef { get; set; }
    public string Status { get; set; }

    public ICollection<SO_Item> SO_Items { get; set; } = new List<SO_Item>();

    public Warehouse Warehouse { get; set; } = null!;
    public int WarehouseId { get; set; }
}

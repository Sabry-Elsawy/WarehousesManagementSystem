namespace WMS.DAL;

public class SalesOrder
{
    public int Id { get; set; }
    public DateTime OrderDate { get; set; }
    public string CustomerRef { get; set; }
    public string Status { get; set; }

    public ICollection<SO_Item> SO_Items { get; set; } = new List<SO_Item>();

    public Warehouse Warehouse { get; set; } = null!;
    public int WarehouseId { get; set; }
}

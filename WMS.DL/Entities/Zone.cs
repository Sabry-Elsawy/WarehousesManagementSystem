namespace WMS.DAL;

public class Zone
{
    public int Id { get; set; }
    public string Name { get; set; }

    //relations
    public int WarehouseId { get; set; }
    public Warehouse Warehouse { get; set; }

    public ICollection<Aisle> Aisles { get; set; }


}

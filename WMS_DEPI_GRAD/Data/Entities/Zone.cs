namespace WMS_DEPI_GRAD.Data.Entities;

public class Zone
{
    public int Id { get; set; }
    public string Name { get; set; }

    //relations
    public int WareHouseId { get; set; }
    public Warehouse Warehouse { get; set; } 

    public ICollection<Aisle> Aisles { get; set; }


}

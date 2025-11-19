namespace WMS_DEPI_GRAD.Data.Entities;

public class Warehouse
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Code { get; set; }
    public int Capacity { get; set; }    
    public string City { get; set; }
    public string Country { get; set; }
    public string Street { get; set; }

    //relations
    public ICollection<Zone> Zones { get; set; }  
}

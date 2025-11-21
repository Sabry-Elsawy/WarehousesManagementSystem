namespace WMS.DAL;

public class Rack
{
    public int Id { get; set; }
    public string Name { get; set; }


    public int AisleId { get; set; }
    public Aisle Aisle { get; set; }

    public ICollection<Bin> Bins{ get; set; }
}

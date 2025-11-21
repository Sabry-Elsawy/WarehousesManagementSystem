namespace WMS.DAL;

public class Product
{
    public int Id { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public double Volume { get; set; }
    public double Weight { get; set; }
    public string UnitOfMeasure { get; set; }

    public ICollection<SO_Item> SO_Items { get; set; }
}

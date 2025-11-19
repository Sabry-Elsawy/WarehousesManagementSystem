namespace WMS_DEPI_GRAD.Data.Entities;

public class Bin
{
    public int Id { get; set; }
    public string Code { get; set; }
    public int Capacity { get; set; }
    public string BinType { get; set; }

    public int RackId { get; set; }
    public Rack Rack { get; set; }

    public ICollection<Picking> Pickings { get; set; }
    public ICollection<Inventory> Inventories { get; set; }
}

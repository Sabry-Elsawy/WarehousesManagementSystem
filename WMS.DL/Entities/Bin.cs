using WMS.DAL.Entities._Common;

namespace WMS.DAL;

public class Bin : BaseAuditableEntity<int>
{
    public string Code { get; set; }
    public int Capacity { get; set; }
    public string BinType { get; set; }

    public int RackId { get; set; }
    public Rack Rack { get; set; }

    public ICollection<Picking> Pickings { get; set; }
    public ICollection<Inventory> Inventories { get; set; }

    public IReadOnlyCollection<PutawayBin> PutawayBins { get; set; } = new List<PutawayBin>();
}

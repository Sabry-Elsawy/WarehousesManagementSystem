using System.Collections.Generic;
using WMS.DAL.Entities._Common;

namespace WMS.DAL.Entities
{
    public class Bin : BaseAuditableEntity<int>
    {
        public string Code { get; set; }
        public int Capacity { get; set; }

        public int RackId { get; set; }
        public Rack Rack { get; set; }

        public int BinTypeId { get; set; }
        public BinType BinType { get; set; }

        public ICollection<Picking> Pickings { get; set; } = new List<Picking>();
        public ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();
        public IReadOnlyCollection<PutawayBin> PutawayBins { get; set; } = new List<PutawayBin>();
    }
}
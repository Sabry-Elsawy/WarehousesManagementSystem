using System.Collections.Generic;
using WMS.DAL.Entities._Common;

namespace WMS.DAL.Entities
{
    public class BinType : BaseAuditableEntity<int>
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public ICollection<Bin> Bins { get; set; } = new List<Bin>();
    }
}
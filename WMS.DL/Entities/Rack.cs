using WMS.DAL.Entities._Common;

namespace WMS.DAL;

public class Rack : BaseAuditableEntity<int>
{
    public string Name { get; set; }


    public int AisleId { get; set; }
    public Aisle Aisle { get; set; }

    public ICollection<Bin> Bins{ get; set; }
}

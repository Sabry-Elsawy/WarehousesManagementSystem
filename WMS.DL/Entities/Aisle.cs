using WMS.DAL.Entities._Common;

namespace WMS.DAL;

public class Aisle : BaseAuditableEntity<int>
{
    public string Name { get; set; }


    public int ZoneId { get; set; }
    public Zone Zone { get; set; }

    public ICollection<Rack> Racks { get; set; }
}

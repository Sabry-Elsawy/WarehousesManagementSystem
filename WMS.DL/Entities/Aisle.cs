using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using WMS.DAL.Entities._Common;

namespace WMS.DAL;

public class Aisle : BaseAuditableEntity<int>
{
    public string Name { get; set; }
    
    //relations
    public int ZoneId { get; set; }
    [ValidateNever]
    public Zone Zone { get; set; }
    [ValidateNever]
    public ICollection<Rack> Racks { get; set; }
}

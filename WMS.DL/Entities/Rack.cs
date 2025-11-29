using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using WMS.DAL.Entities;
using WMS.DAL.Entities._Common;

namespace WMS.DAL;

public class Rack : BaseAuditableEntity<int>
{
    public string Name { get; set; }
    
    //relations
    public int AisleId { get; set; }
    [ValidateNever]
    public Aisle Aisle { get; set; }
    [ValidateNever]
    public ICollection<Bin> Bins { get; set; }
}

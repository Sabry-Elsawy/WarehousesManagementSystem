using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using WMS.DAL.Entities._Common;

namespace WMS.DAL;

public class Zone : BaseAuditableEntity<int>
{
    public string Name { get; set; }

    //relations
    public int WarehouseId { get; set; }
    [ValidateNever]
    public Warehouse Warehouse { get; set; }

    [ValidateNever]
    public ICollection<Aisle> Aisles { get; set; }


}

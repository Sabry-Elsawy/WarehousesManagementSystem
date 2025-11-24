using WMS.DAL.Entities._Common;

namespace WMS.DAL;

public class Zone : BaseAuditableEntity<int>
{
    public string Name { get; set; }

    //relations
    public int WarehouseId { get; set; }
    public Warehouse Warehouse { get; set; }

    public ICollection<Aisle> Aisles { get; set; }


}

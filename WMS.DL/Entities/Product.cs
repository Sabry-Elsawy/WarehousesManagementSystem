using WMS.DAL.Entities._Common;

namespace WMS.DAL;

public class Product : BaseAuditableEntity<int>
{
    public string Code { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public double Volume { get; set; }
    public double Weight { get; set; }
    public string UnitOfMeasure { get; set; }
    public string Barcode { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    public ICollection<SO_Item> SO_Items { get; set; }
}

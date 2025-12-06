using WMS.DAL.Entities._Common;

namespace WMS.DAL.Entities;

public class CycleCountItem : BaseAuditableEntity<int>
{
    public int CycleCountId { get; set; }
    public CycleCount CycleCount { get; set; }

    public int ProductId { get; set; }
    public Product Product { get; set; }

    public int BinId { get; set; }
    public Bin Bin { get; set; }

    public int ExpectedQuantity { get; set; }
    public int CountedQuantity { get; set; }
    public int Difference => CountedQuantity - ExpectedQuantity;
    
    public string Status { get; set; } // Pending, Counted, Approved, Rejected
    public string? Notes { get; set; }
}

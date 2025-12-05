using WMS.DAL.Entities._Common;

namespace WMS.DAL.Entities;

public class CycleCount : BaseAuditableEntity<int>
{
    public string Status { get; set; } // Pending, Counting, Reviewed, Approved, Completed
    public string Description { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    
    public ICollection<CycleCountItem> Items { get; set; } = new List<CycleCountItem>();
}

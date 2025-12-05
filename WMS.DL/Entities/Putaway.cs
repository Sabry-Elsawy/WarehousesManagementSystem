
using System.ComponentModel.DataAnnotations;
using WMS.DAL.Entities._Common;

namespace WMS.DAL;

public class Putaway : BaseAuditableEntity<int>
{
    public int Qty { get; set; }
    
    [ConcurrencyCheck]
    public PutawayStatus Status { get; set; }

    // Workflow timestamps
    public DateTime? AssignedOn { get; set; }
    public DateTime? CompletedOn { get; set; }
    public DateTime? ClosedOn { get; set; }

    // User tracking
    public string? PerformedBy { get; set; }
    public string? ClosedBy { get; set; }

    // Navigation properties
    public int ReceiptItemId { get; set; }
    public ReceiptItem ReceiptItem { get; set; } = null!;
    public IReadOnlyCollection<PutawayBin> PutawayBins { get; set; } = new List<PutawayBin>();

}
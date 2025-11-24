
using WMS.DAL.Entities._Common;

namespace WMS.DAL;

public class Putaway : BaseAuditableEntity<int>
{
    public int Qty { get; set; }
    public PutawayStatus Status { get; set; }



    // Navigation properties
    public int ReceiptItemId { get; set; }
    public ReceiptItem ReceiptItem { get; set; } = null!;
    public IReadOnlyCollection<PutawayBin> PutawayBins { get; set; } = new List<PutawayBin>();

}
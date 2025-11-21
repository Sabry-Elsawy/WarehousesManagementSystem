
namespace WMS.DAL;

public class Putaway
{
    public int Id { get; set; }
    public int Qty { get; set; }
    public PutawayStatus Status { get; set; }



    // Navigation properties
    public int ReceiptItemId { get; set; }
    public ReceiptItem ReceiptItem { get; set; } = null!;
    public IReadOnlyCollection<PutawayBin> PutawayBins { get; set; } = new List<PutawayBin>();

}
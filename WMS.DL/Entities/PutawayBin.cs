using WMS.DAL.Entities._Common;

namespace WMS.DAL;

public class PutawayBin : BaseAuditableEntity<int>
{
    public int PutawayId { get; set; }
    public Putaway Putaway { get; set; } = null!;
    public int BinId { get; set; }
    public Bin Bin { get; set; } = null!;
    public int Qty { get; set; }


}

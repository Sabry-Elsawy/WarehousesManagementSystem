namespace WMS_DEPI_GRAD;

public class PutawayBin
{
    public int Id { get; set; }
    public int PutawayId { get; set; }
    public Putaway Putaway { get; set; } = null!;
    public int BinId { get; set; }
    public Bin Bin { get; set; } = null!;
    public int Qty { get; set; }


}

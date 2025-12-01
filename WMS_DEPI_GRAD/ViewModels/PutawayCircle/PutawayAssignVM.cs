using WMS.DAL.Entities;

namespace WMS_DEPI_GRAD.ViewModels.PutawayCircle;

public class PutawayAssignVM
{
    public int PutawayId { get; set; }

    public string SKU { get; set; }
    public string ProductName { get; set; }
    public int Qty { get; set; }

    public List<BinVM> Bins { get; set; } = new();
}

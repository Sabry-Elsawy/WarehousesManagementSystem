using WMS.DAL.Entities._Common;

namespace WMS.DAL;

public class Picking : BaseAuditableEntity<int>
{
    public int Qty { get; set; }
    public string Status { get; set; }


    public int SO_ItemId { get; set; }
    public SO_Item SO_Item { get; set; }  

    public int ProductId { get; set; }
    public Product Product { get; set; }  

    public int BinId { get; set; }
    public Bin Bin { get; set; }  
}

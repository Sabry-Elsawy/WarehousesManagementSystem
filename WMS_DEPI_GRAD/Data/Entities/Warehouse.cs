
namespace WMS_DEPI_GRAD;

public class Warehouse
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Code { get; set; }
    public int Capacity { get; set; }    
    public string City { get; set; }
    public string Country { get; set; }
    public string Street { get; set; }

    //relations
    public ICollection<Zone> Zones { get; set; }  
    public ICollection<TransferOrder> SourceTransferOrders { get; set; } = new List<TransferOrder>();
    public ICollection<TransferOrder> DestinationTransferOrders { get; set; } = new List<TransferOrder>();
    public IReadOnlyCollection<PurchaseOrder> PurchaseOrders { get; set; } = new List<PurchaseOrder>();
    public IReadOnlyCollection<SalesOrder> SalesOrders { get; set; } = new List<SalesOrder>();
    public IReadOnlyCollection<Receipt> Receipts { get; set; } = new List<Receipt>();
}

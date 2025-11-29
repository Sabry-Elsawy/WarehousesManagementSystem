using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using WMS.DAL.Entities._Common;

namespace WMS.DAL;

public class Warehouse : BaseAuditableEntity<int>
{
    public string Name { get; set; }
    public string Code { get; set; }
    public int Capacity { get; set; }    
    public string City { get; set; }
    public string Country { get; set; }
    public string Street { get; set; }

    //relations
    [ValidateNever]
    public ICollection<Zone> Zones { get; set; }  
    [ValidateNever]
    public ICollection<TransferOrder> SourceTransferOrders { get; set; } = new List<TransferOrder>();
    [ValidateNever]
    public ICollection<TransferOrder> DestinationTransferOrders { get; set; } = new List<TransferOrder>();
    [ValidateNever]
    public IReadOnlyCollection<PurchaseOrder> PurchaseOrders { get; set; } = new List<PurchaseOrder>();
    [ValidateNever]
    public IReadOnlyCollection<SalesOrder> SalesOrders { get; set; } = new List<SalesOrder>();
    [ValidateNever]
    public IReadOnlyCollection<Receipt> Receipts { get; set; } = new List<Receipt>();
}

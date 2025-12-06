using WMS.DAL.Entities._Common;

namespace WMS.DAL.Entities;

public class InventoryTransaction : BaseAuditableEntity<int>
{
    public string TransactionType { get; set; } // Receipt, Putaway, Transfer, Adjustment, CycleCount
    public int QuantityChange { get; set; }
    public string ReferenceNumber { get; set; } // PO Number, TO Number, etc.
    public string Reason { get; set; }
    
    public int ProductId { get; set; }
    public Product Product { get; set; }

    public int? SourceBinId { get; set; }
    public Bin? SourceBin { get; set; }

    public int? DestinationBinId { get; set; }
    public Bin? DestinationBin { get; set; }

    public DateTime TransactionDate { get; set; } = DateTime.UtcNow;
}


namespace WMS.BLL.DTOs;

public class InventoryDetailsDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string SKU { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public int BinId { get; set; }
    public string BinCode { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? BatchNumber { get; set; }
    public string? ExpiryDate { get; set; }
    public DateTime CreatedOn { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? LastModifiedOn { get; set; }
    public string? LastModifiedBy { get; set; }
    public List<InventoryTransactionDto> RecentTransactions { get; set; } = new();
}

public class TransferInventoryDto
{
    public int InventoryId { get; set; }
    public int DestinationBinId { get; set; }
    public int Quantity { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string PerformedBy { get; set; } = string.Empty;
}

public class InventoryTransactionDto
{
    public int Id { get; set; }
    public string TransactionType { get; set; } = string.Empty; // Transfer, Adjustment, Putaway, Receipt
    public int QuantityChange { get; set; }
    public string? SourceBin { get; set; }
    public string? DestinationBin { get; set; }
    public string? Reason { get; set; }
    public DateTime TransactionDate { get; set; }
    public string PerformedBy { get; set; } = string.Empty;
}

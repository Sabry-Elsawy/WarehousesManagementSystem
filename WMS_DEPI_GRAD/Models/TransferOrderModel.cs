namespace WMS_DEPI_GRAD.Models;

public class TransferOrderModel
{
    public int Id { get; set; }
    public string TransferId { get; set; } = string.Empty; 
    public string SKU { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public string FromLocation { get; set; } = string.Empty;
    public string ToLocation { get; set; } = string.Empty;
    public string Status { get; set; } = "pending"; 
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
}
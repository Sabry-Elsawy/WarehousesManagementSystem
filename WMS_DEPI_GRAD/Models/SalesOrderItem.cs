namespace WMS_DEPI_GRAD.Models;

public class SalesOrderItem
{
    public string SKU { get; set; } = null!;
    public int SalesOrderId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Qty { get; set; }
    
}
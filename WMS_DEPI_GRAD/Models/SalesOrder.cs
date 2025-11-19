namespace WMS_DEPI_GRAD.Models;

public class SalesOrder
{
    public int Id { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public List<SalesOrderItem> Items { get; set; } = new List<SalesOrderItem>();

}
    
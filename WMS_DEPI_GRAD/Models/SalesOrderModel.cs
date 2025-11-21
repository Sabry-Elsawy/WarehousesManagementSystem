namespace WMS_DEPI_GRAD.Models;

public class SalesOrderModel
{
    public int Id { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public List<SalesOrderItemModel> Items { get; set; } = new List<SalesOrderItemModel>();

}
    
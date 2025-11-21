namespace WMS.DAL;

public class SO_Item
{
    public int Id { get; set; }
    public int SalesOrderId { get; set; } // FK 
    public int ProductId { get; set; }    // FK 
    public int QtyOrdered { get; set; }
    public int QtyPicked { get; set; }
    public string Status { get; set; }

    // Navigation properties
    public SalesOrder SalesOrder { get; set; }
    public Product Product { get; set; }
}

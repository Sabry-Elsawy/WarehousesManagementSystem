namespace WMS_DEPI_GRAD;

public class Inventory
{
    public int Id { get; set; }
    public string Status { get; set; }
    public int Quantity { get; set; }
    public string BatchNumber { get; set; }
    public string ExpiryDate { get; set; }

    public int ProductId { get; set; }
    public Product Product { get; set; }

    public int BinId { get; set; }
    public Bin Bin { get; set; }
}

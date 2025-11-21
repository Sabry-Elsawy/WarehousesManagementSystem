namespace WMS.DAL;

public class ReceiptItem
{
    public int Id { get; set; }
    public int ExpectedQty { get; set; }
    public int ReceivedQty { get; set; }
    public string SKU { get; set; } = null!;



    // Navigation Properties
    public int ReceiptId { get; set; }
    public Receipt Receipt { get; set; } = null!;
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public IReadOnlyCollection<Putaway> Putaways { get; set; } = new List<Putaway>();


}
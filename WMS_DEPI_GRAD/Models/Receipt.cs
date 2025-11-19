namespace WMS_DEPI_GRAD.Models;

public class Receipt
{
    public int Id { get; set; }

    public int POId { get; set; }
    public int? ASNId { get; set; }
    public string VendorName { get; set; } = string.Empty;
    public DateTime ReceiptDate { get; set; }
    public List<ReceiptItem> Items { get; set; } = new List<ReceiptItem>();

}

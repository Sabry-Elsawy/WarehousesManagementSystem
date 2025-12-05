

namespace WMS.BLL.DTOs;
public class InventoryListDto
{
    public int Id { get; set; }
    public string SKU { get; set; }
    public string ProductName { get; set; }
    public int Quantity { get; set; }
    public string Location { get; set; }
    public string Status { get; set; }
    public DateTime LastUpdated { get; set; }
}

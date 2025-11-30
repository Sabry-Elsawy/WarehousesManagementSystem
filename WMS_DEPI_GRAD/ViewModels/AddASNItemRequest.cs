namespace WMS_DEPI_GRAD.ViewModels;

public class AddASNItemRequest
{
    public int AsnId { get; set; }
    public ASNItemDto Item { get; set; }
}

public class ASNItemDto
{
    public int ProductId { get; set; }
    public int QtyShipped { get; set; }
    public int? LinkedPOItemId { get; set; }
}

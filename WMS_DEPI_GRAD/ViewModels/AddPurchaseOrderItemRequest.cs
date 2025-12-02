namespace WMS_DEPI_GRAD.ViewModels;

public class AddPurchaseOrderItemRequest
{
    public int PoId { get; set; }
    public PurchaseOrderItemDto Item { get; set; }
}

public class PurchaseOrderItemDto
{
    public int ProductId { get; set; }
    public int QtyOrdered { get; set; }
    public decimal UnitPrice { get; set; }
}

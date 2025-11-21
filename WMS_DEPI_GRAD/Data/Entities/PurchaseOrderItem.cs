
namespace WMS_DEPI_GRAD
{
    public class PurchaseOrderItem
    {
        public int Id { get; set; }
        public int Qty { get; set; }
        public string SKU { get; set; } = null!;


        // Navigation Properties
        public int PurchaseOrderId { get; set; }
        public PurchaseOrder PurchaseOrder { get; set; } = null!;

        public Product Product { get; set; } = null!;
        public int ProductId { get; set; }

    }
}
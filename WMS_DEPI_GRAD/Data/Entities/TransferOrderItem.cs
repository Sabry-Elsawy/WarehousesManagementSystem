namespace WMS_DEPI_GRAD
{
    public class TransferOrderItem
    {
        public int Id { get; set; }
        public int Qty { get; set; }



        //Navigation Properties

        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;

        public TransferOrder TransferOrder { get; set; } = null!;   
        public int TransferOrderId { get; set; }
    }
}
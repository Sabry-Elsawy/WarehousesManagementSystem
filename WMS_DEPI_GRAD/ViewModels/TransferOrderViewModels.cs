using System.ComponentModel.DataAnnotations;
using WMS.DAL;

namespace WMS_DEPI_GRAD.ViewModels
{
    public class TransferOrderViewModel
    {
        public int Id { get; set; }
        public string Status { get; set; }
        public DateTime CreatedOn { get; set; }
        public string SourceWarehouse { get; set; }
        public string DestinationWarehouse { get; set; }
        public int ItemCount { get; set; }
    }

    public class CreateTransferOrderViewModel
    {
        [Required(ErrorMessage = "Source Warehouse is required")]
        [Display(Name = "Source Warehouse")]
        public int SourceWarehouseId { get; set; }

        [Required(ErrorMessage = "Destination Warehouse is required")]
        [Display(Name = "Destination Warehouse")]
        public int DestinationWarehouseId { get; set; }

        public List<TransferOrderItemViewModel> Items { get; set; } = new List<TransferOrderItemViewModel>();
    }

    public class TransferOrderItemViewModel
    {
        [Required]
        public int ProductId { get; set; }
        
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }
    }

    public class TransferOrderDetailsViewModel
    {
        public int Id { get; set; }
        public string Status { get; set; }
        public DateTime CreatedOn { get; set; }
        public string SourceWarehouse { get; set; }
        public string DestinationWarehouse { get; set; }
        public List<TransferOrderItemDetailsViewModel> Items { get; set; } = new();
    }

    public class TransferOrderItemDetailsViewModel
    {
        public string ProductName { get; set; }
        public string SKU { get; set; }
        public string Code { get; set; }
        public int Quantity { get; set; }
    }
}

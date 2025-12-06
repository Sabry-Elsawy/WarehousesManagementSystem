using System.ComponentModel.DataAnnotations;
using WMS.DAL;

namespace WMS_DEPI_GRAD.ViewModels;

public class TransferOrderViewModel
{
    public int Id { get; set; }
    public TransferOrderStatus Status { get; set; }
    public DateTime CreatedOn { get; set; }

    [Required(ErrorMessage = "Source Warehouse is required")]
    [Display(Name = "Source Warehouse")]
    public int SourceWarehouseId { get; set; }
    public string SourceWarehouseName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Destination Warehouse is required")]
    [Display(Name = "Destination Warehouse")]
    public int DestinationWarehouseId { get; set; }
    public string DestinationWarehouseName { get; set; } = string.Empty;

    public List<TransferOrderItemViewModel> Items { get; set; } = new();
}

public class TransferOrderItemViewModel
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductCode { get; set; } = string.Empty;
    public int Qty { get; set; }
}

public class CreateTransferOrderViewModel
{
    [Required]
    [Display(Name = "Source Warehouse")]
    public int SourceWarehouseId { get; set; }

    [Required]
    [Display(Name = "Destination Warehouse")]
    public int DestinationWarehouseId { get; set; }
}

public class AddTransferItemViewModel
{
    [Required]
    public int TransferOrderId { get; set; }

    [Required]
    [Display(Name = "Product")]
    public int ProductId { get; set; }

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
    public int Quantity { get; set; }
}

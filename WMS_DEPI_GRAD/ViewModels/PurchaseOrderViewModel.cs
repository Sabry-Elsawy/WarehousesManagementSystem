using System.ComponentModel.DataAnnotations;
using WMS.DAL;

namespace WMS_DEPI_GRAD.ViewModels;

public class PurchaseOrderViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "PO Number is required")]
    [Display(Name = "PO Number")]
    public string PO_Number { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vendor is required")]
    [Display(Name = "Vendor")]
    public int VendorId { get; set; }
    public string VendorName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Warehouse is required")]
    [Display(Name = "Warehouse")]
    public int WarehouseId { get; set; }
    public string WarehouseName { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Order Date")]
    [DataType(DataType.Date)]
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;

    [Required]
    [Display(Name = "Expected Arrival Date")]
    [DataType(DataType.Date)]
    public DateTime ExpectedArrivalDate { get; set; }

    public PurchaseOrderStatus Status { get; set; }
    public string StatusDisplay => Status.ToString();

    public List<PurchaseOrderItemViewModel> Items { get; set; } = new();

    public DateTime CreatedOn { get; set; }
    public string? CreatedBy { get; set; }
    public string? CreatedByName { get; set; } // Added for User Name display
    public DateTime? LastModifiedOn { get; set; }
    public string? LastModifiedBy { get; set; }
    public string? LastModifiedByName { get; set; } // Added for User Name display
}

public class PurchaseOrderItemViewModel
{
    public int Id { get; set; }

    [Required]
    [Display(Name = "Product")]
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;

    public string SKU { get; set; } = string.Empty;

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
    [Display(Name = "Quantity Ordered")]
    public int QtyOrdered { get; set; }

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Unit Price must be greater than 0")]
    [Display(Name = "Unit Price")]
    [DataType(DataType.Currency)]
    public decimal UnitPrice { get; set; }

    [Display(Name = "Quantity Received")]
    public int QtyReceived { get; set; }

    public string LineStatus { get; set; } = "Open";

    [Display(Name = "Total")]
    public decimal LineTotal => QtyOrdered * UnitPrice;
}

public class CreatePurchaseOrderViewModel
{
    [Required(ErrorMessage = "PO Number is required")]
    [Display(Name = "PO Number")]
    public string PO_Number { get; set; } = $"PO-{DateTime.Now:yyyyMMddHHmmss}";

    [Required(ErrorMessage = "Vendor is required")]
    [Display(Name = "Vendor")]
    public int VendorId { get; set; }

    [Required(ErrorMessage = "Warehouse is required")]
    [Display(Name = "Warehouse")]
    public int WarehouseId { get; set; }

    [Required]
    [Display(Name = "Expected Arrival Date")]
    [DataType(DataType.Date)]
    public DateTime ExpectedArrivalDate { get; set; } = DateTime.UtcNow.AddDays(7);
}

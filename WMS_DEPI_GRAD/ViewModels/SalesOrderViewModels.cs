using System.ComponentModel.DataAnnotations;
using WMS.DAL;

namespace WMS_DEPI_GRAD.ViewModels;

public class SalesOrderViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "SO Number is required")]
    [Display(Name = "SO Number")]
    public string SO_Number { get; set; } = string.Empty;

    [Required(ErrorMessage = "Customer is required")]
    [Display(Name = "Customer")]
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Warehouse is required")]
    [Display(Name = "Warehouse")]
    public int WarehouseId { get; set; }
    public string WarehouseName { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Order Date")]
    [DataType(DataType.Date)]
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;

    public SalesOrderStatus Status { get; set; }
    public string StatusDisplay => Status.ToString();

    public List<SalesOrderItemViewModel> Items { get; set; } = new();

    public DateTime CreatedOn { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? LastModifiedOn { get; set; }
    public string? LastModifiedBy { get; set; }
}

public class CreateSalesOrderViewModel
{
    [Required(ErrorMessage = "SO Number is required")]
    [Display(Name = "SO Number")]
    public string SO_Number { get; set; } = $"SO-{DateTime.Now:yyyyMMddHHmmss}";

    [Required(ErrorMessage = "Customer is required")]
    [Display(Name = "Customer")]
    public int CustomerId { get; set; }

    [Required(ErrorMessage = "Warehouse is required")]
    [Display(Name = "Warehouse")]
    public int WarehouseId { get; set; }

    [Required]
    [Display(Name = "Order Date")]
    [DataType(DataType.Date)]
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
}

public class SalesOrderItemViewModel
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

    [Display(Name = "Quantity Picked")]
    public int QtyPicked { get; set; }

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Unit Price must be greater than 0")]
    [Display(Name = "Unit Price")]
    [DataType(DataType.Currency)]
    public decimal UnitPrice { get; set; }

    [Display(Name = "Total")]
    public decimal LineTotal => QtyOrdered * UnitPrice;
}

// Request model for AJAX AddItem endpoint
public class AddSalesOrderItemRequest
{
    public int SoId { get; set; }
    public SalesOrderItemViewModel Item { get; set; } = null!;
}

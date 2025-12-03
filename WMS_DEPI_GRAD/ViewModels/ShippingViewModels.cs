using System.ComponentModel.DataAnnotations;

namespace WMS_DEPI_GRAD.ViewModels;

public class ShippingViewModel
{
    public int SalesOrderId { get; set; }

    [Display(Name = "SO Number")]
    public string SO_Number { get; set; } = string.Empty;

    [Display(Name = "Customer")]
    public string CustomerName { get; set; } = string.Empty;

    [Display(Name = "Warehouse")]
    public string WarehouseName { get; set; } = string.Empty;

    [Display(Name = "Order Date")]
    [DataType(DataType.Date)]
    public DateTime OrderDate { get; set; }

    [Display(Name = "Items Count")]
    public int ItemCount { get; set; }

    [Display(Name = "Total Quantity")]
    public int TotalQuantity { get; set; }
}

public class ShippingConfirmationViewModel
{
    public int SalesOrderId { get; set; }

    [Display(Name = "SO Number")]
    public string SO_Number { get; set; } = string.Empty;

    [Display(Name = "Customer")]
    public string CustomerName { get; set; } = string.Empty;

    [Display(Name = "Warehouse")]
    public string WarehouseName { get; set; } = string.Empty;

    [Display(Name = "Order Date")]
    [DataType(DataType.Date)]
    public DateTime OrderDate { get; set; }

    [Display(Name = "Delivery Note Number")]
    public string DeliveryNoteNumber { get; set; } = string.Empty;

    public List<ShippingItemViewModel> Items { get; set; } = new();
}

public class ShippingItemViewModel
{
    public string ProductName { get; set; } = string.Empty;
    public string SKU { get; set; } = string.Empty;
    public int QtyOrdered { get; set; }
    public int QtyPicked { get; set; }
}

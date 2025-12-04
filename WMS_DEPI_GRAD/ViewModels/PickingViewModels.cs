using System.ComponentModel.DataAnnotations;
using WMS.DAL;

namespace WMS_DEPI_GRAD.ViewModels;

public class PickingTaskViewModel
{
    public int Id { get; set; }

    [Display(Name = "Sales Order")]
    public string SO_Number { get; set; } = string.Empty;

    public int SalesOrderId { get; set; }

    [Display(Name = "Customer")]
    public string CustomerName { get; set; } = string.Empty;

    [Display(Name = "Product")]
    public string ProductName { get; set; } = string.Empty;

    public string SKU { get; set; } = string.Empty;

    [Display(Name = "Bin Location")]
    public string BinLocation { get; set; } = string.Empty;

    [Display(Name = "Quantity to Pick")]
    public int QuantityToPick { get; set; }

    [Display(Name = "Quantity Picked")]
    public int QuantityPicked { get; set; }

    public PickingStatus Status { get; set; }
    public string StatusDisplay => Status.ToString();

    public DateTime CreatedOn { get; set; }
}

public class ConfirmPickingViewModel
{
    public int PickingTaskId { get; set; }

    [Display(Name = "Bin Location")]
    public string BinLocation { get; set; } = string.Empty;

    [Display(Name = "Product")]
    public string ProductName { get; set; } = string.Empty;

    public string SKU { get; set; } = string.Empty;

    [Display(Name = "Quantity to Pick")]
    public int QuantityToPick { get; set; }

    [Required]
    [Range(0, int.MaxValue, ErrorMessage = "Quantity picked cannot be negative")]
    [Display(Name = "Quantity Picked")]
    public int QuantityPicked { get; set; }
}

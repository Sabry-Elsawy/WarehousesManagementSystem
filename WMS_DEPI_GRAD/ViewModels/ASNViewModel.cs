using System.ComponentModel.DataAnnotations;
using WMS.DAL;

namespace WMS_DEPI_GRAD.ViewModels;

public class ASNViewModel
{
    public int Id { get; set; }

    [Display(Name = "ASN Number")]
    public string ASN_Number { get; set; } = string.Empty;

    [Display(Name = "Purchase Order")]
    public int PurchaseOrderId { get; set; }
    public string PO_Number { get; set; } = string.Empty;

    [Display(Name = "Expected Arrival")]
    [DataType(DataType.Date)]
    public DateTime ExpectedArrivalDate { get; set; }

    [Display(Name = "Tracking Number")]
    public string TrackingNumber { get; set; } = string.Empty;

    public AdvancedShippingNoticeStatus Status { get; set; }
    public string StatusDisplay => Status.ToString();

    public List<ASNItemViewModel> Items { get; set; } = new();

    public DateTime CreatedOn { get; set; }
    public string? CreatedBy { get; set; }

    public List<PurchaseOrderItemViewModel> POItems { get; set; } = new();
}

public class ASNItemViewModel
{
    public int Id { get; set; }

    [Display(Name = "Product")]
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;

    public string SKU { get; set; } = string.Empty;

    [Display(Name = "Quantity Shipped")]
    public int QtyShipped { get; set; }

    [Display(Name = "Qty Ordered (PO)")]
    public int? QtyOrdered { get; set; }

    public int? LinkedPOItemId { get; set; }
}

public class CreateASNViewModel
{
    [Required]
    [Display(Name = "Purchase Order")]
    public int PurchaseOrderId { get; set; }

    [Required]
    [Display(Name = "ASN Number")]
    public string ASN_Number { get; set; } = $"ASN-{DateTime.Now:yyyyMMddHHmmss}";

    [Display(Name = "Tracking Number")]
    [StringLength(100)]
    public string TrackingNumber { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Expected Arrival")]
    [DataType(DataType.Date)]
    public DateTime ExpectedArrivalDate { get; set; } = DateTime.UtcNow.AddDays(5);
}

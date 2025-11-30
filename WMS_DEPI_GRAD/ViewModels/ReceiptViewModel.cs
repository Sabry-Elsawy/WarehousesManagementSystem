using System.ComponentModel.DataAnnotations;
using WMS.DAL;

namespace WMS_DEPI_GRAD.ViewModels;

public class ReceiptViewModel
{
    public int Id { get; set; }

    [Display(Name = "Receipt Number")]
    public string ReceiptNumber { get; set; } = string.Empty;

    [Display(Name = "ASN")]
    public int AdvancedShippingNoticeId { get; set; }
    public string ASN_Number { get; set; } = string.Empty;

    [Display(Name = "Warehouse")]
    public int WarehouseId { get; set; }
    public string WarehouseName { get; set; } = string.Empty;

    [Display(Name = "Received Date")]
    [DataType(DataType.Date)]
    public DateTime ReceivedDate { get; set; }

    public ReceiptStatus Status { get; set; }
    public string StatusDisplay => Status.ToString();

    public List<ReceiptItemViewModel> Items { get; set; } = new();

    public DateTime CreatedOn { get; set; }
    public string? CreatedBy { get; set; }

    // Summary properties
    public int TotalItems => Items.Count;
    public int ProcessedItems => Items.Count(i => i.QtyReceived > 0);
    public int ItemsWithDiscrepancy => Items.Count(i => i.DiscrepancyType != DiscrepancyType.None);
}

public class ReceiptItemViewModel
{
    public int Id { get; set; }

    public int ReceiptId { get; set; }

    public int ASNItemId { get; set; }

    [Display(Name = "Product")]
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;

    public string SKU { get; set; } = string.Empty;

    [Display(Name = "Expected Qty")]
    public int QtyExpected { get; set; }

    [Display(Name = "Received Qty")]
    public int QtyReceived { get; set; }

    public DiscrepancyType DiscrepancyType { get; set; }
    public string DiscrepancyDisplay => DiscrepancyType.ToString();

    [Display(Name = "Notes")]
    public string? Notes { get; set; }

    public int Variance => QtyReceived - QtyExpected;
    public bool HasDiscrepancy => DiscrepancyType != DiscrepancyType.None;
}

public class CreateReceiptViewModel
{
    [Required]
    [Display(Name = "ASN")]
    public int ASNId { get; set; }

    [Required]
    [Display(Name = "Warehouse")]
    public int WarehouseId { get; set; }
}

public class ScanItemViewModel
{
    [Required]
    public int ReceiptId { get; set; }

    [Required]
    [Display(Name = "Product Code / Barcode")]
    public string ProductCodeOrBarcode { get; set; } = string.Empty;

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
    [Display(Name = "Quantity")]
    public int Qty { get; set; } = 1;
}

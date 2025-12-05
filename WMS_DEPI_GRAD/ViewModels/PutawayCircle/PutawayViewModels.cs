using WMS.DAL;
using WMS.BLL.DTOs;

namespace WMS_DEPI_GRAD.ViewModels.PutawayCircle;

/// <summary>
/// ViewModel for Putaway Index page with pagination
/// </summary>
public class PutawayIndexViewModel
{
    public List<PutawayListVM> Items { get; set; } = new();
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public int TotalCount { get; set; }
    public string? SearchTerm { get; set; }
    public string? StatusFilter { get; set; }

    public bool HasPreviousPage => CurrentPage > 1;
    public bool HasNextPage => CurrentPage < TotalPages;
}

/// <summary>
/// ViewModel for Putaway Details page
/// </summary>
public class PutawayDetailsViewModel
{
    // Putaway information
    public int PutawayId { get; set; }
    public int Qty { get; set; }
    public PutawayStatus Status { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime? AssignedOn { get; set; }
    public DateTime? CompletedOn { get; set; }
    public DateTime? ClosedOn { get; set; }
    public string? CreatedBy { get; set; }
    public string? PerformedBy { get; set; }
    public string? ClosedBy { get; set; }
    
    // Receipt Item details
    public string SKU { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string? BatchNumber { get; set; }
    public string? ExpiryDate { get; set; }
    
    // Assigned bins
    public List<PutawayBinDetailVM> AssignedBins { get; set; } = new();
    
    // Current inventory summary
    public List<InventorySummaryDto> InventorySummary { get; set; } = new();
}

public class PutawayBinDetailVM
{
    public int BinId { get; set; }
    public string BinCode { get; set; } = string.Empty;
    public int AssignedQty { get; set; }
    public int CurrentInventoryQty { get; set; }
    public int AvailableCapacity { get; set; }
    public string Zone { get; set; } = string.Empty;
    public string Aisle { get; set; } = string.Empty;
}

/// <summary>
/// ViewModel for Create Putaway page
/// </summary>
public class CreatePutawayViewModel
{
    public List<ReceiptItemOption> AvailableReceiptItems { get; set; } = new();
    public int SelectedReceiptItemId { get; set; }
    public int Qty { get; set; }
}

public class ReceiptItemOption
{
    public int ReceiptItemId { get; set; }
    public string SKU { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public int AvailableQty { get; set; }
    public string DisplayText => $"{SKU} - {ProductName} (Available: {AvailableQty})";
}

/// <summary>
/// ViewModel for Auto-Assign Bins page
/// </summary>
public class AutoAssignViewModel
{
    public int PutawayId { get; set; }
    public string SKU { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public int TotalQty { get; set; }
    public int AssignedQty { get; set; }
    public bool FullyAssigned { get; set; }
    public List<PutawayBinDto> SuggestedBins { get; set; } = new();
    public string? WarningMessage { get; set; }
}

/// <summary>
/// ViewModel for Manual Bin Assignment page
/// </summary>
public class AssignManualViewModel
{
    public int PutawayId { get; set; }
    public string SKU { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public int TotalQty { get; set; }
    public List<BinCapacityDto> AvailableBins { get; set; } = new();
    public List<PutawayBinDto> CurrentAssignments { get; set; } = new();
}

/// <summary>
/// ViewModel for Execute Putaway confirmation page
/// </summary>
public class ExecutePutawayViewModel
{
    public int PutawayId { get; set; }
    public string SKU { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public int TotalQty { get; set; }
    public List<PutawayBinDto> Assignments { get; set; } = new();
    public List<InventorySummaryDto> BeforeInventory { get; set; } = new();
    public List<InventorySummaryDto> AfterInventory { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
}

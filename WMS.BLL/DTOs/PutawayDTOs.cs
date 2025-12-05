using WMS.DAL;

namespace WMS.BLL.DTOs;

/// <summary>
/// DTO for creating a new putaway task
/// </summary>
public class CreatePutawayDto
{
    public int ReceiptItemId { get; set; }
    public int Qty { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
}

/// <summary>
/// Complete putaway data transfer object
/// </summary>
public class PutawayDto
{
    public int Id { get; set; }
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
    public int ReceiptItemId { get; set; }
    public string SKU { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public int ProductId { get; set; }
    public string? BatchNumber { get; set; }
    public string? ExpiryDate { get; set; }
    
    // Assigned bins
    public List<PutawayBinDto> AssignedBins { get; set; } = new();
}

/// <summary>
/// Bin assignment details
/// </summary>
public class PutawayBinDto
{
    public int PutawayBinId { get; set; }
    public int BinId { get; set; }
    public string BinCode { get; set; } = string.Empty;
    public int Qty { get; set; }
    public int AvailableCapacity { get; set; }
    public string BinType { get; set; } = string.Empty;
    public string Zone { get; set; } = string.Empty;
}

/// <summary>
/// Result from auto-assign bins operation
/// </summary>
public class AutoAssignResultDto
{
    public int PutawayId { get; set; }
    public int TotalQty { get; set; }
    public int AssignedQty { get; set; }
    public bool FullyAssigned => AssignedQty >= TotalQty;
    public List<PutawayBinDto> SuggestedBins { get; set; } = new();
    public string? WarningMessage { get; set; }
}

/// <summary>
/// Bin capacity information
/// </summary>
public class BinCapacityDto
{
    public int BinId { get; set; }
    public string Code { get; set; } = string.Empty;
    public int TotalCapacity { get; set; }
    public int UsedCapacity { get; set; }
    public int AvailableCapacity => TotalCapacity - UsedCapacity;
    public string BinTypeName { get; set; } = string.Empty;
    public int BinTypeId { get; set; }
    public string ZoneName { get; set; } = string.Empty;
    public int ZoneId { get; set; }
    public int CurrentProductQty { get; set; } // Qty of same product already in bin
}

/// <summary>
/// Pagination filter for list queries
/// </summary>
public class PaginationFilter
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
    public string? StatusFilter { get; set; }
}

/// <summary>
/// Inventory summary for a bin
/// </summary>
public class InventorySummaryDto
{
    public int BinId { get; set; }
    public string BinCode { get; set; } = string.Empty;
    public int CurrentQty { get; set; }
    public int ProjectedQty { get; set; }
}

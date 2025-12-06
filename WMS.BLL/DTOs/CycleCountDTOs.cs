namespace WMS.BLL.DTOs;

public class CycleCountDto
{
    public int Id { get; set; }
    public string Status { get; set; }
    public string Description { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime CreatedOn { get; set; }
    public string CreatedBy { get; set; }
    public int TotalItems { get; set; }
    public int TotalDiscrepancies { get; set; }
}

public class CycleCountItemDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string SKU { get; set; }
    public string ProductName { get; set; }
    public int BinId { get; set; }
    public string BinCode { get; set; }
    public int ExpectedQuantity { get; set; }
    public int CountedQuantity { get; set; }
    public int Difference { get; set; }
    public string Status { get; set; }
    public string? Notes { get; set; }
}

public class CreateCycleCountDto
{
    public string Description { get; set; }
    public List<int>? ProductIds { get; set; } // Optional filter
    public List<int>? BinIds { get; set; } // Optional filter
    public int? ZoneId { get; set; } // Optional filter
}

public class UpdateCountDto
{
    public int ItemId { get; set; }
    public int CountedQuantity { get; set; }
    public string? Notes { get; set; }
}

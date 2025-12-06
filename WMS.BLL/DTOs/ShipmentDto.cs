namespace WMS.BLL.DTOs;

public class ShipmentDto
{
    public string Carrier { get; set; } = string.Empty;
    public string TrackingNumber { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public string PerformedBy { get; set; } = string.Empty;
}

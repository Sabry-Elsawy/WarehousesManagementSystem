namespace WMS.BLL.DTOs;

public class BinDTO
{
    public int BinId { get; set; }
    public string Code { get; set; }
    public int Capacity { get; set; }
    public int Used { get; set; }

    public int Available => Capacity - Used;

    public bool HasEnoughSpace(int requiredQty)
        => Available >= requiredQty;
}

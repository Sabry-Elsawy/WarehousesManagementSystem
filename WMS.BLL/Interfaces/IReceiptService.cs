using WMS.DAL;

namespace WMS.BLL.Interfaces;

public interface IReceiptService
{
    Task<Receipt?> GetByIdAsync(int id);
    Task<IEnumerable<Receipt>> GetAllAsync();
    Task<Receipt> CreateFromASNAsync(int asnId, int warehouseId);
    Task<bool> ScanItemAsync(int receiptId, string productCodeOrBarcode, int qty);
    Task<bool> CloseAsync(int receiptId, string closedBy, string? reason = null);
    Task<bool> ForceCloseAsync(int receiptId, string closedBy, string reason);
    Task<bool> HandleDiscrepancyAsync(int receiptItemId, string notes);
    Task<ReceiptItem?> GetReceiptItemByIdAsync(int receiptItemId);
}

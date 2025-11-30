using WMS.DAL;

namespace WMS.BLL.Interfaces;

public interface IReceiptService
{
    Task<Receipt?> GetByIdAsync(int id);
    Task<IEnumerable<Receipt>> GetAllAsync();
    Task<Receipt> CreateFromASNAsync(int asnId, int warehouseId);
    Task<bool> ScanItemAsync(int receiptId, string productCodeOrBarcode, int qty);
    Task<bool> CloseAsync(int receiptId);
    Task<bool> HandleDiscrepancyAsync(int receiptItemId, string notes);
    Task<ReceiptItem?> GetReceiptItemByIdAsync(int receiptItemId);
}

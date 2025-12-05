using Microsoft.EntityFrameworkCore;
using WMS.BLL.Interfaces;
using WMS.DAL;
using WMS.DAL.UnitOfWork;

namespace WMS.BLL.Services;

public class ReceiptService : IReceiptService
{
    private readonly IUnitOfWork _unitOfWork;

    public ReceiptService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Receipt?> GetByIdAsync(int id)
    {
        var repo = _unitOfWork.GetRepository<Receipt, int>();
        return await repo.GetByIdAsync(id, query => query
            .Include(r => r.ReceiptItems)
                .ThenInclude(ri => ri.Product)
            .Include(r => r.AdvancedShippingNotice)
            .Include(r => r.Warehouse));
    }

    public async Task<IEnumerable<Receipt>> GetAllAsync()
    {
        var repo = _unitOfWork.GetRepository<Receipt, int>();
        return await repo.GetAllWithIncludeAsync(withTracking: false, query => query
            .Include(r => r.AdvancedShippingNotice)
            .Include(r => r.Warehouse));
    }

    public async Task<Receipt> CreateFromASNAsync(int asnId, int warehouseId)
    {
        // Check if a Receipt already exists for this ASN - if so, return it instead of creating a new one
        var receiptRepo = _unitOfWork.GetRepository<Receipt, int>();
        var existingReceipts = await receiptRepo.GetAllWithIncludeAsync(withTracking: false, 
            query => query.Where(r => r.AdvancedShippingNoticeId == asnId));
        
        if (existingReceipts.Any())
        {
            // Return the existing receipt instead of throwing an error
            var existingReceipt = existingReceipts.First();
            return existingReceipt;
        }

        // Validate ASN exists
        var asnRepo = _unitOfWork.GetRepository<AdvancedShippingNotice, int>();
        var asn = await asnRepo.GetByIdAsync(asnId, query => query.Include(a => a.ASNItems));

        if (asn == null)
            throw new InvalidOperationException("ASN not found");

        if (asn.Status != AdvancedShippingNoticeStatus.Received)
            throw new InvalidOperationException("Receipt can only be created from Received ASNs");

        // Create receipt
        var receipt = new Receipt
        {
            ReceiptNumber = $"RCP-{DateTime.UtcNow:yyyyMMddHHmmss}",
            AdvancedShippingNoticeId = asnId,
            WarehouseId = warehouseId,
            ReceivedDate = DateTime.UtcNow,
            Status = ReceiptStatus.Open
        };

        await receiptRepo.AddAsync(receipt);
        await _unitOfWork.CompleteAsync();

        var receiptItemRepo = _unitOfWork.GetRepository<ReceiptItem, int>();
        foreach (var asnItem in asn.ASNItems)
        {
            var receiptItem = new ReceiptItem
            {
                ReceiptId = receipt.Id,
                ASNItemId = asnItem.Id,
                ProductId = asnItem.ProductId,
                QtyExpected = asnItem.QtyShipped,
                QtyReceived = 0,
                SKU = asnItem.SKU,
                DiscrepancyType = DiscrepancyType.None
            };

            await receiptItemRepo.AddAsync(receiptItem);
        }

        await _unitOfWork.CompleteAsync();

        return receipt;
    }

    public async Task<bool> ScanItemAsync(int receiptId, string productCodeOrBarcode, int qty)
    {
        if (qty <= 0)
            throw new ArgumentException("Quantity must be greater than 0");

        // Get the receipt
        var receiptRepo = _unitOfWork.GetRepository<Receipt, int>();
        var receipt = await receiptRepo.GetByIdAsync(receiptId);

        if (receipt == null)
            throw new InvalidOperationException("Receipt not found");

        if (receipt.Status == ReceiptStatus.Closed)
            throw new InvalidOperationException("Cannot scan items for a closed receipt");

        // Find product by SKU or code efficiently
        var productRepo = _unitOfWork.GetRepository<Product, int>();
        var products = await productRepo.GetAllAsync(WithTracking: false);
        var product = products.FirstOrDefault(p => p.Code == productCodeOrBarcode);

        if (product == null)
            throw new InvalidOperationException($"Product not found with code: {productCodeOrBarcode}");

        // Find the receipt item for this product efficiently
        var receiptItemRepo = _unitOfWork.GetRepository<ReceiptItem, int>();
        var receiptItems = await receiptItemRepo.GetAllWithIncludeAsync(withTracking: true, query => query
            .Where(ri => ri.ReceiptId == receiptId && ri.ProductId == product.Id));
        
        var receiptItem = receiptItems.FirstOrDefault();

        if (receiptItem == null)
            throw new InvalidOperationException($"Product {productCodeOrBarcode} is not expected in this receipt");

        // Update qty received
        receiptItem.QtyReceived += qty;

        // Determine discrepancy
        if (receiptItem.QtyReceived < receiptItem.QtyExpected)
        {
            receiptItem.DiscrepancyType = DiscrepancyType.Shortage;
        }
        else if (receiptItem.QtyReceived > receiptItem.QtyExpected)
        {
            receiptItem.DiscrepancyType = DiscrepancyType.Overage;
        }
        else
        {
            receiptItem.DiscrepancyType = DiscrepancyType.None;
        }

        receiptItemRepo.Update(receiptItem);

        // Update PO item QtyReceived
        var asnItemRepo = _unitOfWork.GetRepository<AdvancedShippingNoticeItem, int>();
        var asnItem = await asnItemRepo.GetByIdAsync(receiptItem.ASNItemId);
        
        if (asnItem != null && asnItem.LinkedPOItemId.HasValue)
        {
            var poItemRepo = _unitOfWork.GetRepository<PurchaseOrderItem, int>();
            var poItem = await poItemRepo.GetByIdAsync(asnItem.LinkedPOItemId.Value);
            
            if (poItem != null)
            {
                poItem.QtyReceived += qty;
                poItemRepo.Update(poItem);
            }
        }
        
        await _unitOfWork.CompleteAsync();

        return true;
    }

    public async Task<bool> CloseAsync(int receiptId)
    {
        var receiptRepo = _unitOfWork.GetRepository<Receipt, int>();
        var receipt = await receiptRepo.GetByIdAsync(receiptId);

        if (receipt == null)
            throw new InvalidOperationException("Receipt not found");

        if (receipt.Status == ReceiptStatus.Closed)
            throw new InvalidOperationException("Receipt is already closed");

        // Verify all items have been processed
        var receiptItemRepo = _unitOfWork.GetRepository<ReceiptItem, int>();
        var allItems = await receiptItemRepo.GetAllAsync(WithTracking: false);
        var items = allItems.Where(ri => ri.ReceiptId == receiptId);

        if (items.Any(i => i.QtyReceived == 0))
            throw new InvalidOperationException("Cannot close receipt with unprocessed items");

        receipt.Status = ReceiptStatus.Closed;
        receiptRepo.Update(receipt);
        await _unitOfWork.CompleteAsync();

        // Trigger auto-close for ASN if all receipts are closed
        await TryAutoCloseASNAsync(receiptId);

        return true;
    }

    public async Task<bool> HandleDiscrepancyAsync(int receiptItemId, string notes)
    {
        var repo = _unitOfWork.GetRepository<ReceiptItem, int>();
        var item = await repo.GetByIdAsync(receiptItemId);

        if (item == null)
            throw new InvalidOperationException("Receipt item not found");

        if (item.DiscrepancyType == DiscrepancyType.None)
            throw new InvalidOperationException("No discrepancy to handle");

        item.Notes = notes;
        repo.Update(item);
        await _unitOfWork.CompleteAsync();

        return true;
    }

    public async Task<ReceiptItem?> GetReceiptItemByIdAsync(int receiptItemId)
    {
        var repo = _unitOfWork.GetRepository<ReceiptItem, int>();
        return await repo.GetByIdAsync(receiptItemId);
    }

    /// <summary>
    /// Attempts to automatically close an ASN if all its receipts are closed.
    /// </summary>
    private async Task TryAutoCloseASNAsync(int receiptId)
    {
        try
        {
            // Get the receipt to find its ASN
            var receiptRepo = _unitOfWork.GetRepository<Receipt, int>();
            var receipt = await receiptRepo.GetByIdAsync(receiptId);
            
            if (receipt == null || receipt.AdvancedShippingNoticeId == 0)
                return; // Receipt not linked to ASN

            int asnId = receipt.AdvancedShippingNoticeId;

            // Get the ASN with tracking enabled for update
            var asnRepo = _unitOfWork.GetRepository<AdvancedShippingNotice, int>();
            var asn = await asnRepo.GetByIdAsync(asnId);

            if (asn == null || asn.Status == AdvancedShippingNoticeStatus.Closed)
                return; // ASN already closed or not found

            // Check if there are any other open receipts for this ASN
            var allReceiptsList = await receiptRepo.GetAllWithIncludeAsync(withTracking: false,
                query => query.Where(r => r.AdvancedShippingNoticeId == asnId));

            if (allReceiptsList.Any(r => r.Status != ReceiptStatus.Closed))
                return; // Some receipts still open

            // All receipts are closed - auto-close the ASN
            asn.Status = AdvancedShippingNoticeStatus.Closed;
            asnRepo.Update(asn);
            await _unitOfWork.CompleteAsync();

            // Log auto-closure (in production, use proper logging framework)
            Console.WriteLine($"[AUTO-CLOSE] ASN {asn.Id} ({asn.ASN_Number}) automatically closed - all receipts completed");

            // Cascade: Trigger PO auto-close check by calling ASNService's TryAutoClosePOAsync indirectly
            // We need to trigger the ASN's close logic which will check PO
            // Since we just changed ASN status, we should trigger PO check
            await TryAutoClosePOAsync(asnId);
        }
        catch (Exception ex)
        {
            // Don't fail the receipt closure if ASN auto-close fails
            Console.WriteLine($"[WARNING] Failed to auto-close ASN for Receipt {receiptId}: {ex.Message}");
        }
    }

    /// <summary>
    /// Helper to trigger PO auto-close when ASN is auto-closed.
    /// This replicates the logic that would be in ASNService but keeps ReceiptService independent.
    /// </summary>
    private async Task TryAutoClosePOAsync(int asnId)
    {
        try
        {
            var asnRepo = _unitOfWork.GetRepository<AdvancedShippingNotice, int>();
            var asn = await asnRepo.GetByIdAsync(asnId);
            
            if (asn == null || asn.PurchaseOrderId == 0)
                return;

            int poId = asn.PurchaseOrderId;

            var poRepo = _unitOfWork.GetRepository<PurchaseOrder, int>();
            var po = await poRepo.GetByIdAsync(poId, 
                query => query.Include(p => p.POItems),
                withTracking: true);

            if (po == null || po.Status == PurchaseOrderStatus.Closed)
                return;

            if (po.POItems.Any(item => item.QtyReceived < item.QtyOrdered))
                return;

            var allAsnsList = await asnRepo.GetAllWithIncludeAsync(withTracking: false,
                query => query.Where(a => a.PurchaseOrderId == poId));

            if (allAsnsList.Any(a => a.Status != AdvancedShippingNoticeStatus.Closed))
                return;

            po.Status = PurchaseOrderStatus.Closed;
            poRepo.Update(po);
            await _unitOfWork.CompleteAsync();

            Console.WriteLine($"[AUTO-CLOSE] PO {po.Id} ({po.PO_Number}) automatically closed - all items received and all ASNs closed");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[WARNING] Failed to auto-close PO for ASN {asnId}: {ex.Message}");
        }
    }
}

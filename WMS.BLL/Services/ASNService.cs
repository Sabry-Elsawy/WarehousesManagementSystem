using Microsoft.EntityFrameworkCore;
using WMS.BLL.Interfaces;
using WMS.DAL;
using WMS.DAL.UnitOfWork;

namespace WMS.BLL.Services;

public class ASNService : IASNService
{
    private readonly IUnitOfWork _unitOfWork;

    public ASNService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<AdvancedShippingNotice?> GetByIdAsync(int id)
    {
        var repo = _unitOfWork.GetRepository<AdvancedShippingNotice, int>();
        return await repo.GetByIdAsync(id, query => query
            .Include(asn => asn.PurchaseOrder)
                .ThenInclude(po => po.POItems)
                    .ThenInclude(poi => poi.Product)
            .Include(asn => asn.ASNItems)
                .ThenInclude(item => item.Product));
    }

    public async Task<IEnumerable<AdvancedShippingNotice>> GetAllAsync()
    {
        var repo = _unitOfWork.GetRepository<AdvancedShippingNotice, int>();
        return await repo.GetAllWithIncludeAsync(withTracking: false, query => query
            .Include(asn => asn.PurchaseOrder));
    }

    public async Task<AdvancedShippingNotice> CreateFromPOAsync(int purchaseOrderId, AdvancedShippingNotice asn)
    {
        // Validate PO exists and is approved
        var poRepo = _unitOfWork.GetRepository<PurchaseOrder, int>();
        var po = await poRepo.GetByIdAsync(purchaseOrderId);

        if (po == null)
            throw new InvalidOperationException("Purchase Order not found");

        if (po.Status != PurchaseOrderStatus.Approved)
            throw new InvalidOperationException("ASN can only be created from Approved Purchase Orders");

        // Set ASN properties
        asn.PurchaseOrderId = purchaseOrderId;
        asn.Status = AdvancedShippingNoticeStatus.Sent;

        if (string.IsNullOrWhiteSpace(asn.ASN_Number))
            throw new ArgumentException("ASN Number is required");

        var asnRepo = _unitOfWork.GetRepository<AdvancedShippingNotice, int>();
        await asnRepo.AddAsync(asn);
        await _unitOfWork.CompleteAsync();

        return asn;
    }

    public async Task<AdvancedShippingNotice> UpdateAsync(AdvancedShippingNotice asn)
    {
        if (asn.Id <= 0)
            throw new ArgumentException("Invalid ASN ID");

        var repo = _unitOfWork.GetRepository<AdvancedShippingNotice, int>();
        var existing = await repo.GetByIdAsync(asn.Id);

        if (existing == null)
            throw new InvalidOperationException("ASN not found");

        if (existing.Status == AdvancedShippingNoticeStatus.Closed)
            throw new InvalidOperationException("Cannot update a closed ASN");

        repo.Update(asn);
        await _unitOfWork.CompleteAsync();

        return asn;
    }

    public async Task<bool> MarkReceivedAsync(int asnId)
    {
        var repo = _unitOfWork.GetRepository<AdvancedShippingNotice, int>();
        var asn = await repo.GetByIdAsync(asnId);

        if (asn == null)
            throw new InvalidOperationException("ASN not found");

        if (asn.Status != AdvancedShippingNoticeStatus.Sent)
            throw new InvalidOperationException("Only Sent ASNs can be marked as Received");

        asn.Status = AdvancedShippingNoticeStatus.Received;
        repo.Update(asn);
        await _unitOfWork.CompleteAsync();

        return true;
    }

    public async Task<bool> CloseAsync(int asnId)
    {
        var repo = _unitOfWork.GetRepository<AdvancedShippingNotice, int>();
        var asn = await repo.GetByIdAsync(asnId);

        if (asn == null)
            throw new InvalidOperationException("ASN not found");

        if (asn.Status != AdvancedShippingNoticeStatus.Received)
            throw new InvalidOperationException("Only Received ASNs can be closed");

        // Validate that a Receipt exists for this ASN
        var receiptRepo = _unitOfWork.GetRepository<Receipt, int>();
        var receipts = await receiptRepo.GetAllWithIncludeAsync(withTracking: false,
            query => query.Where(r => r.AdvancedShippingNoticeId == asnId));

        if (!receipts.Any())
            throw new InvalidOperationException("Cannot close ASN: A Receipt must be created from this ASN before it can be closed. Please create and process a Receipt first.");

        var receipt = receipts.First();

        // Validate that the Receipt is closed
        if (receipt.Status != ReceiptStatus.Closed)
            throw new InvalidOperationException($"Cannot close ASN: The associated Receipt (ID: {receipt.Id}, Number: {receipt.ReceiptNumber}) must be closed first. Current Receipt status: {receipt.Status}");

        // All validations passed, close the ASN
        asn.Status = AdvancedShippingNoticeStatus.Closed;
        repo.Update(asn);
        await _unitOfWork.CompleteAsync();

        return true;
    }

    public async Task<bool> AddItemAsync(int asnId, AdvancedShippingNoticeItem item)
    {
        // Validation
        if (item.QtyShipped <= 0)
            throw new ArgumentException("Quantity shipped must be greater than 0");

        if (item.ProductId <= 0)
            throw new ArgumentException("Product is required");

        var asnRepo = _unitOfWork.GetRepository<AdvancedShippingNotice, int>();
        var asn = await asnRepo.GetByIdAsync(asnId);

        if (asn == null)
            throw new InvalidOperationException("ASN not found");

        if (asn.Status == AdvancedShippingNoticeStatus.Closed)
            throw new InvalidOperationException("Cannot add items to a closed ASN");

        // Validate against PO items if LinkedPOItemId is provided
        if (item.LinkedPOItemId.HasValue)
        {
            var poItemRepo = _unitOfWork.GetRepository<PurchaseOrderItem, int>();
            var poItem = await poItemRepo.GetByIdAsync(item.LinkedPOItemId.Value);

            if (poItem == null || poItem.PurchaseOrderId != asn.PurchaseOrderId)
                throw new InvalidOperationException("Invalid PO Item reference");

            // Allow partial shipments but validate qty
            if (item.QtyShipped > poItem.QtyOrdered)
                throw new InvalidOperationException("Shipped quantity cannot exceed ordered quantity");
        }

        item.AdvancedShippingNoticeId = asnId;

        var itemRepo = _unitOfWork.GetRepository<AdvancedShippingNoticeItem, int>();
        await itemRepo.AddAsync(item);
        await _unitOfWork.CompleteAsync();

        return true;
    }
}

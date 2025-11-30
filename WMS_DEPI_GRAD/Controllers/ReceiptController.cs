using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WMS.BLL.Interfaces;
using WMS.DAL;
using WMS.DAL.UnitOfWork;

namespace WMS_DEPI_GRAD.Controllers;

[Authorize]
public class ReceiptController : Controller
{
    private readonly IReceiptService _receiptService;
    private readonly IASNService _asnService;
    private readonly IUnitOfWork _unitOfWork;

    public ReceiptController(IReceiptService receiptService, IASNService asnService, IUnitOfWork unitOfWork)
    {
        _receiptService = receiptService;
        _asnService = asnService;
        _unitOfWork = unitOfWork;
    }

    public async Task<IActionResult> Index()
    {
        var receipts = await _receiptService.GetAllAsync();
        return View(receipts);
    }

    public async Task<IActionResult> Details(int id)
    {
        var receipt = await _receiptService.GetByIdAsync(id);
        if (receipt == null)
            return NotFound();

        return View(receipt);
    }

    [Authorize(Roles = "Warehouse,Admin")]
    public async Task<IActionResult> Create(int? asnId)
    {
        await LoadLookups();
        
        if (asnId.HasValue)
            ViewBag.SelectedASNId = asnId.Value;

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Warehouse,Admin")]
    public async Task<IActionResult> Create(int asnId, int warehouseId)
    {
        try
        {
            var receipt = await _receiptService.CreateFromASNAsync(asnId, warehouseId);
            TempData["Success"] = "Receipt created successfully!";
            return RedirectToAction(nameof(Details), new { id = receipt.Id });
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
            await LoadLookups();
            return View();
        }
    }

    [Authorize(Roles = "Warehouse,Admin")]
    public async Task<IActionResult> Scan(int id)
    {
        var receipt = await _receiptService.GetByIdAsync(id);
        if (receipt == null)
            return NotFound();

        return View(receipt);
    }

    [HttpPost]
    [Route("api/receipt/scan")]
    public async Task<IActionResult> ScanItem([FromBody] ScanRequest request)
    {
        try
        {
            await _receiptService.ScanItemAsync(request.ReceiptId, request.ProductCodeOrBarcode, request.Qty);
            
            // Get updated receipt item to return details
            var receipt = await _receiptService.GetByIdAsync(request.ReceiptId);
            
            return Json(new
            {
                success = true,
                message = "Item scanned successfully",
                product = request.ProductCodeOrBarcode,
                qty = request.Qty
            });
        }
        catch (Exception ex)
        {
            return Json(new
            {
                success = false,
                message = ex.Message
            });
        }
    }

    [HttpPost]
    [Authorize(Roles = "Warehouse,Admin")]
    public async Task<IActionResult> Close(int id)
    {
        try
        {
            await _receiptService.CloseAsync(id);
            TempData["Success"] = "Receipt closed successfully!";
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [Authorize(Roles = "Warehouse,Admin")]
    public async Task<IActionResult> HandleDiscrepancy(int receiptItemId, string notes)
    {
        try
        {
            await _receiptService.HandleDiscrepancyAsync(receiptItemId, notes);
            var item = await _receiptService.GetReceiptItemByIdAsync(receiptItemId);
            
            return Json(new
            {
                success = true,
                message = "Discrepancy handled successfully"
            });
        }
        catch (Exception ex)
        {
            return Json(new
            {
                success = false,
                message = ex.Message
            });
        }
    }

    private async Task LoadLookups()
    {
        var asns = await _asnService.GetAllAsync();
        var receivedASNs = asns.Where(a => a.Status == AdvancedShippingNoticeStatus.Received);
        
        ViewBag.ASNs = new SelectList(receivedASNs, "Id", "ASN_Number");

        var warehouseRepo = _unitOfWork.GetRepository<Warehouse, int>();
        ViewBag.Warehouses = new SelectList(await warehouseRepo.GetAllAsync(), "Id", "Name");
    }
}

public class ScanRequest
{
    public int ReceiptId { get; set; }
    public string ProductCodeOrBarcode { get; set; } = string.Empty;
    public int Qty { get; set; }
}

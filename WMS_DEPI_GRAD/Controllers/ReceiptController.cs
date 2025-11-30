using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WMS.BLL.Interfaces;
using WMS.DAL;
using WMS.DAL.UnitOfWork;

namespace WMS_DEPI_GRAD.Controllers;

//[Authorize]
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
        var viewModels = receipts.Select(r => new ReceiptViewModel
        {
            Id = r.Id,
            ReceiptNumber = r.ReceiptNumber,
            AdvancedShippingNoticeId = r.AdvancedShippingNoticeId,
            ASN_Number = r.AdvancedShippingNotice?.ASN_Number ?? "",
            WarehouseId = r.WarehouseId,
            WarehouseName = r.Warehouse?.Name ?? "",
            ReceivedDate = r.ReceivedDate,
            Status = r.Status,
            CreatedOn = r.CreatedOn,
            CreatedBy = r.CreatedBy,
            Items = r.ReceiptItems.Select(i => new ReceiptItemViewModel
            {
                Id = i.Id,
                ReceiptId = i.ReceiptId,
                ASNItemId = i.ASNItemId,
                ProductId = i.ProductId,
                ProductName = i.Product?.Name ?? "",
                SKU = i.Product?.Code ?? "",
                QtyExpected = i.QtyExpected,
                QtyReceived = i.QtyReceived,
                DiscrepancyType = i.DiscrepancyType,
                Notes = i.Notes
            }).ToList()
        }).ToList();

        return View(viewModels);
    }

    public async Task<IActionResult> Details(int id)
    {
        var receipt = await _receiptService.GetByIdAsync(id);
        if (receipt == null)
            return NotFound();

        var viewModel = new ReceiptViewModel
        {
            Id = receipt.Id,
            ReceiptNumber = receipt.ReceiptNumber,
            AdvancedShippingNoticeId = receipt.AdvancedShippingNoticeId,
            ASN_Number = receipt.AdvancedShippingNotice?.ASN_Number ?? "",
            WarehouseId = receipt.WarehouseId,
            WarehouseName = receipt.Warehouse?.Name ?? "",
            ReceivedDate = receipt.ReceivedDate,
            Status = receipt.Status,
            CreatedOn = receipt.CreatedOn,
            CreatedBy = receipt.CreatedBy,
            Items = receipt.ReceiptItems.Select(i => new ReceiptItemViewModel
            {
                Id = i.Id,
                ReceiptId = i.ReceiptId,
                ASNItemId = i.ASNItemId,
                ProductId = i.ProductId,
                ProductName = i.Product?.Name ?? "",
                SKU = i.Product?.Code ?? "",
                QtyExpected = i.QtyExpected,
                QtyReceived = i.QtyReceived,
                DiscrepancyType = i.DiscrepancyType,
                Notes = i.Notes
            }).ToList()
        };

        return View(viewModel);
    }

    //[Authorize(Roles = "Warehouse,Admin")]
    public async Task<IActionResult> Create(int? asnId)
    {
        await LoadLookups();
        
        var viewModel = new CreateReceiptViewModel();
        if (asnId.HasValue)
            viewModel.ASNId = asnId.Value;

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    //[Authorize(Roles = "Warehouse,Admin")]
    public async Task<IActionResult> Create(CreateReceiptViewModel viewModel)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var receipt = await _receiptService.CreateFromASNAsync(viewModel.ASNId, viewModel.WarehouseId);
                TempData["Success"] = "Receipt created successfully!";
                return RedirectToAction(nameof(Details), new { id = receipt.Id });
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
        }

        await LoadLookups();
        return View(viewModel);
    }

    //[Authorize(Roles = "Warehouse,Admin")]
    public async Task<IActionResult> Scan(int id)
    {
        var receipt = await _receiptService.GetByIdAsync(id);
        if (receipt == null)
            return NotFound();

        var viewModel = new ReceiptViewModel
        {
            Id = receipt.Id,
            ReceiptNumber = receipt.ReceiptNumber,
            AdvancedShippingNoticeId = receipt.AdvancedShippingNoticeId,
            ASN_Number = receipt.AdvancedShippingNotice?.ASN_Number ?? "",
            WarehouseId = receipt.WarehouseId,
            WarehouseName = receipt.Warehouse?.Name ?? "",
            ReceivedDate = receipt.ReceivedDate,
            Status = receipt.Status,
            CreatedOn = receipt.CreatedOn,
            CreatedBy = receipt.CreatedBy,
            Items = receipt.ReceiptItems.Select(i => new ReceiptItemViewModel
            {
                Id = i.Id,
                ReceiptId = i.ReceiptId,
                ASNItemId = i.ASNItemId,
                ProductId = i.ProductId,
                ProductName = i.Product?.Name ?? "",
                SKU = i.Product?.Code ?? "",
                QtyExpected = i.QtyExpected,
                QtyReceived = i.QtyReceived,
                DiscrepancyType = i.DiscrepancyType,
                Notes = i.Notes
            }).ToList()
        };

        return View(viewModel);
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
    //[Authorize(Roles = "Warehouse,Admin")]
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
    //[Authorize(Roles = "Warehouse,Admin")]
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

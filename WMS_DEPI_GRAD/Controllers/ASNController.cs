using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WMS.BLL.Interfaces;
using WMS.DAL;
using WMS.DAL.UnitOfWork;

namespace WMS_DEPI_GRAD.Controllers;

[Authorize]
public class ASNController : Controller
{
    private readonly IASNService _asnService;
    private readonly IPurchaseOrderService _poService;
    private readonly IUnitOfWork _unitOfWork;

    public ASNController(IASNService asnService, IPurchaseOrderService poService, IUnitOfWork unitOfWork)
    {
        _asnService = asnService;
        _poService = poService;
        _unitOfWork = unitOfWork;
    }

    public async Task<IActionResult> Index()
    {
        var asns = await _asnService.GetAllAsync();
        return View(asns);
    }

    public async Task<IActionResult> Details(int id)
    {
        var asn = await _asnService.GetByIdAsync(id);
        if (asn == null)
            return NotFound();

        return View(asn);
    }

    [Authorize(Roles = "Procurement,Admin")]
    public async Task<IActionResult> Create(int? poId)
    {
        await LoadLookups();
        
        if (poId.HasValue)
            ViewBag.SelectedPOId = poId.Value;

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Procurement,Admin")]
    public async Task<IActionResult> Create(int purchaseOrderId, AdvancedShippingNotice asn)
    {
        if (ModelState.IsValid)
        {
            try
            {
                await _asnService.CreateFromPOAsync(purchaseOrderId, asn);
                TempData["Success"] = "ASN created successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
        }

        await LoadLookups();
        return View(asn);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var asn = await _asnService.GetByIdAsync(id);
        if (asn == null)
            return NotFound();

        await LoadLookups();
        return View(asn);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Procurement,Admin")]
    public async Task<IActionResult> Edit(int id, AdvancedShippingNotice asn)
    {
        if (id != asn.Id)
            return BadRequest();

        if (ModelState.IsValid)
        {
            try
            {
                await _asnService.UpdateAsync(asn);
                TempData["Success"] = "ASN updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
        }

        await LoadLookups();
        return View(asn);
    }

    [HttpPost]
    [Authorize(Roles = "Warehouse,Admin")]
    public async Task<IActionResult> MarkReceived(int id)
    {
        try
        {
            await _asnService.MarkReceivedAsync(id);
            TempData["Success"] = "ASN marked as received!";
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [Authorize(Roles = "Warehouse,Admin")]
    public async Task<IActionResult> Close(int id)
    {
        try
        {
            await _asnService.CloseAsync(id);
            TempData["Success"] = "ASN closed successfully!";
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [Authorize(Roles = "Procurement,Admin")]
    public async Task<IActionResult> AddItem(int asnId, AdvancedShippingNoticeItem item)
    {
        try
        {
            await _asnService.AddItemAsync(asnId, item);
            return Json(new { success = true, message = "Item added successfully" });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    private async Task LoadLookups()
    {
        var pos = await _poService.GetAllAsync();
        var approvedPOs = pos.Where(p => p.Status == PurchaseOrderStatus.Approved);
        
        ViewBag.PurchaseOrders = new SelectList(approvedPOs, "Id", "PO_Number");

        var productRepo = _unitOfWork.GetRepository<Product, int>();
        ViewBag.Products = new SelectList(await productRepo.GetAllAsync(), "Id", "Name");
    }
}

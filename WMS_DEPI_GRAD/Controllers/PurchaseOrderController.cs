using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WMS.BLL.Interfaces;
using WMS.DAL;
using WMS.DAL.UnitOfWork;

namespace WMS_DEPI_GRAD.Controllers;

[Authorize]
public class PurchaseOrderController : Controller
{
    private readonly IPurchaseOrderService _poService;
    private readonly IUnitOfWork _unitOfWork;

    public PurchaseOrderController(IPurchaseOrderService poService, IUnitOfWork unitOfWork)
    {
        _poService = poService;
        _unitOfWork = unitOfWork;
    }

    public async Task<IActionResult> Index()
    {
        var purchaseOrders = await _poService.GetAllAsync();
        return View(purchaseOrders);
    }

    public async Task<IActionResult> Details(int id)
    {
        var po = await _poService.GetByIdAsync(id);
        if (po == null)
            return NotFound();

        return View(po);
    }

    public async Task<IActionResult> Create()
    {
        await LoadLookups();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Procurement,Admin")]
    public async Task<IActionResult> Create(PurchaseOrder purchaseOrder)
    {
        if (ModelState.IsValid)
        {
            try
            {
                await _poService.CreateAsync(purchaseOrder);
                TempData["Success"] = "Purchase Order created successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
        }

        await LoadLookups();
        return View(purchaseOrder);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var po = await _poService.GetByIdAsync(id);
        if (po == null)
            return NotFound();

        await LoadLookups();
        return View(po);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Procurement,Admin")]
    public async Task<IActionResult> Edit(int id, PurchaseOrder purchaseOrder)
    {
        if (id != purchaseOrder.Id)
            return BadRequest();

        if (ModelState.IsValid)
        {
            try
            {
                await _poService.UpdateAsync(purchaseOrder);
                TempData["Success"] = "Purchase Order updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
        }

        await LoadLookups();
        return View(purchaseOrder);
    }

    [HttpPost]
    [Authorize(Roles = "Procurement,Admin")]
    public async Task<IActionResult> Approve(int id)
    {
        try
        {
            await _poService.ApproveAsync(id);
            TempData["Success"] = "Purchase Order approved successfully!";
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [Authorize(Roles = "Procurement,Admin")]
    public async Task<IActionResult> Close(int id)
    {
        try
        {
            await _poService.CloseAsync(id);
            TempData["Success"] = "Purchase Order closed successfully!";
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [Authorize(Roles = "Procurement,Admin")]
    public async Task<IActionResult> AddItem(int poId, PurchaseOrderItem item)
    {
        try
        {
            await _poService.AddItemAsync(poId, item);
            return Json(new { success = true, message = "Item added successfully" });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    private async Task LoadLookups()
    {
        var vendorRepo = _unitOfWork.GetRepository<Vendor, int>();
        var warehouseRepo = _unitOfWork.GetRepository<Warehouse, int>();
        var productRepo = _unitOfWork.GetRepository<Product, int>();

        ViewBag.Vendors = new SelectList(await vendorRepo.GetAllAsync(), "Id", "Name");
        ViewBag.Warehouses = new SelectList(await warehouseRepo.GetAllAsync(), "Id", "Name");
        ViewBag.Products = new SelectList(await productRepo.GetAllAsync(), "Id", "Name");
    }
}

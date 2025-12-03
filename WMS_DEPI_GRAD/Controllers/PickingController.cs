using Microsoft.AspNetCore.Mvc;
using WMS.BLL.Interfaces;
using WMS.DAL;
using WMS.DAL.UnitOfWork;
using WMS_DEPI_GRAD.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace WMS_DEPI_GRAD.Controllers;

//[Authorize]
public class PickingController : Controller
{
    private readonly IPickingService _pickingService;
    private readonly IUnitOfWork _unitOfWork;

    public PickingController(IPickingService pickingService, IUnitOfWork unitOfWork)
    {
        _pickingService = pickingService;
        _unitOfWork = unitOfWork;
    }

    public async Task<IActionResult> Index()
    {
        var pickings = await _pickingService.GetAllAsync();

        var viewModels = pickings.Select(p => new PickingTaskViewModel
        {
            Id = p.Id,
            SO_Number = p.SO_Item?.SalesOrder?.SO_Number ?? "N/A",
            SalesOrderId = p.SO_Item?.SalesOrderId ?? 0,
            CustomerName = p.SO_Item?.SalesOrder?.Customer?.Name ?? "N/A",
            ProductName = p.Product?.Name ?? "",
            SKU = p.Product?.Code ?? "",
            BinLocation = p.Bin?.BinLocation ?? "",
            QuantityToPick = p.QuantityToPick,
            QuantityPicked = p.QuantityPicked,
            Status = p.Status,
            CreatedOn = p.CreatedOn
        }).ToList();

        return View(viewModels);
    }

    public async Task<IActionResult> Details(int salesOrderId)
    {
        var pickings = await _pickingService.GetBySalesOrderIdAsync(salesOrderId);

        // Get Sales Order details
        var soRepo = _unitOfWork.GetRepository<SalesOrder, int>();
        var salesOrder = await soRepo.GetByIdAsync(
            salesOrderId,
            include: q => q.Include(s => s.Customer).Include(s => s.Warehouse));

        if (salesOrder == null)
            return NotFound();

        ViewBag.SalesOrder = new SalesOrderViewModel
        {
            Id = salesOrder.Id,
            SO_Number = salesOrder.SO_Number,
            CustomerName = salesOrder.Customer?.Name ?? "",
            WarehouseName = salesOrder.Warehouse?.Name ?? "",
            OrderDate = salesOrder.OrderDate,
            Status = salesOrder.Status
        };

        var viewModels = pickings.Select(p => new PickingTaskViewModel
        {
            Id = p.Id,
            SO_Number = salesOrder.SO_Number,
            SalesOrderId = salesOrderId,
            CustomerName = salesOrder.Customer?.Name ?? "",
            ProductName = p.Product?.Name ?? "",
            SKU = p.Product?.Code ?? "",
            BinLocation = p.Bin?.BinLocation ?? "",
            QuantityToPick = p.QuantityToPick,
            QuantityPicked = p.QuantityPicked,
            Status = p.Status,
            CreatedOn = p.CreatedOn
        }).ToList();

        return View(viewModels);
    }

    public async Task<IActionResult> Confirm(int id)
    {
        var picking = await _pickingService.GetByIdAsync(id);
        if (picking == null)
            return NotFound();

        var viewModel = new ConfirmPickingViewModel
        {
            PickingTaskId = picking.Id,
            BinLocation = picking.Bin?.BinLocation ?? "",
            ProductName = picking.Product?.Name ?? "",
            SKU = picking.Product?.Code ?? "",
            QuantityToPick = picking.QuantityToPick,
            QuantityPicked = 0
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Confirm(int id, ConfirmPickingViewModel viewModel)
    {
        if (ModelState.IsValid)
        {
            try
            {
                await _pickingService.ConfirmPickingAsync(id, viewModel.QuantityPicked);
                TempData["Success"] = "Picking confirmed successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
        }

        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Cancel(int id)
    {
        try
        {
            await _pickingService.CancelPickingAsync(id);
            TempData["Success"] = "Picking task cancelled successfully!";
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }
}

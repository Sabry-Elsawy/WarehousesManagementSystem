using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using WMS.BLL.Interfaces;
using WMS.DAL;

namespace WMS_DEPI_GRAD.Controllers;

[Authorize(Roles = "Admin,WarehouseManager")]
public class TransferOrderController : Controller
{
    private readonly ITransferOrderService _transferService;
    private readonly IWarehouseService _warehouseService;
    private readonly IProductService _productService;

    public TransferOrderController(
        ITransferOrderService transferService,
        IWarehouseService warehouseService,
        IProductService productService)
    {
        _transferService = transferService;
        _warehouseService = warehouseService;
        _productService = productService;
    }

    public async Task<IActionResult> Index()
    {
        var orders = await _transferService.GetAllAsync();
        var model = orders.Select(o => new TransferOrderViewModel
        {
            Id = o.Id,
            SourceWarehouseId = o.SourceWarehouseId,
            SourceWarehouseName = o.SourceWarehouse?.Name ?? "Unknown",
            DestinationWarehouseId = o.DestinationWarehouseId,
            DestinationWarehouseName = o.DestinationWarehouse?.Name ?? "Unknown",
            Status = o.Status,
            CreatedOn = o.CreatedOn,
            Items = o.TransferOrderItems.Select(i => new TransferOrderItemViewModel
            {
                Id = i.Id,
                ProductId = i.ProductId,
                ProductName = i.Product?.Name ?? "",
                Qty = i.Qty
            }).ToList()
        }).ToList();

        return View(model);
    }

    public async Task<IActionResult> Details(int id)
    {
        var order = await _transferService.GetByIdAsync(id);
        if (order == null) return NotFound();

        var model = new TransferOrderViewModel
        {
            Id = order.Id,
            SourceWarehouseId = order.SourceWarehouseId,
            SourceWarehouseName = order.SourceWarehouse?.Name ?? "Unknown",
            DestinationWarehouseId = order.DestinationWarehouseId,
            DestinationWarehouseName = order.DestinationWarehouse?.Name ?? "Unknown",
            Status = order.Status,
            CreatedOn = order.CreatedOn,
            Items = order.TransferOrderItems.Select(i => new TransferOrderItemViewModel
            {
                Id = i.Id,
                ProductId = i.ProductId,
                ProductName = i.Product?.Name ?? "",
                ProductCode = i.Product?.Code ?? "",
                Qty = i.Qty
            }).ToList()
        };

        ViewBag.Products = new SelectList(await _productService.GetAllAsync(), "Id", "Name");
        return View(model);
    }

    public async Task<IActionResult> Create()
    {
        var warehouses = await _warehouseService.GetAllWarehousesAsync();
        ViewBag.Warehouses = new SelectList(warehouses, "Id", "Name");
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateTransferOrderViewModel model)
    {
        if (ModelState.IsValid)
        {
            if (model.SourceWarehouseId == model.DestinationWarehouseId)
            {
                ModelState.AddModelError("", "Source and Destination warehouses cannot be the same.");
            }
            else
            {
                var order = new TransferOrder
                {
                    SourceWarehouseId = model.SourceWarehouseId,
                    DestinationWarehouseId = model.DestinationWarehouseId,
                    Status = TransferOrderStatus.Pending
                };

                await _transferService.CreateAsync(order);
                return RedirectToAction(nameof(Details), new { id = order.Id });
            }
        }

        var warehouses = await _warehouseService.GetAllWarehousesAsync();
        ViewBag.Warehouses = new SelectList(warehouses, "Id", "Name");
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> AddItem(AddTransferItemViewModel model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                await _transferService.AddItemAsync(model.TransferOrderId, model.ProductId, model.Quantity);
                TempData["Success"] = "Item added successfully.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
        }
        else
        {
            TempData["Error"] = "Invalid data provided.";
        }

        return RedirectToAction(nameof(Details), new { id = model.TransferOrderId });
    }

    [HttpPost]
    public async Task<IActionResult> Issue(int id)
    {
        try
        {
            await _transferService.IssueAsync(id);
            TempData["Success"] = "Order Issued successfully. Inventory deducted from Source.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
        }
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    public async Task<IActionResult> Receive(int id)
    {
        try
        {
            await _transferService.ReceiveAsync(id);
            TempData["Success"] = "Order Received successfully. Inventory added to Destination.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
        }
        return RedirectToAction(nameof(Details), new { id });
    }
    
    [HttpPost]
    public async Task<IActionResult> Cancel(int id)
    {
        try
        {
            await _transferService.CancelAsync(id);
            TempData["Success"] = "Order Cancelled.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
        }
        return RedirectToAction(nameof(Details), new { id });
    }
}

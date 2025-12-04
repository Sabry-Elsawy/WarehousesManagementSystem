using Microsoft.AspNetCore.Mvc;
using WMS.BLL.Interfaces;
using WMS.DAL;
using WMS_DEPI_GRAD.Models;

namespace WMS_DEPI_GRAD.Controllers;

public class TransferOrdersController : Controller
{
    private readonly ITransferOrderService _transferOrderService;
    private readonly IWarehouseService _warehouseService;
    private readonly IProductService _productService;

    public TransferOrdersController(
        ITransferOrderService transferOrderService,
        IWarehouseService warehouseService,
        IProductService productService)
    {
        _transferOrderService = transferOrderService;
        _warehouseService = warehouseService;
        _productService = productService;
    }

    public async Task<IActionResult> Index()
    {
        var warehouses = await _warehouseService.GetAllWarehousesAsync();
        var products = await _productService.GetAllAsync();
        ViewBag.Warehouses = warehouses;
        ViewBag.Products = products;

        var orders = await _transferOrderService.GetAllAsync();
        var model = orders.Select(o => new TransferOrderModel
        {
            Id = o.Id,
            TransferId = $"TO-{o.CreatedOn.Year}-{o.Id:D3}",
            SKU = o.TransferOrderItems.FirstOrDefault()?.Product.Code ?? "N/A",
            Quantity = o.TransferOrderItems.FirstOrDefault()?.Qty ?? 0,
            FromLocation = o.SourceWarehouse?.Name ?? "Unknown",
            ToLocation = o.DestinationWarehouse?.Name ?? "Unknown",
            Status = o.Status.ToString().ToLower(),
            CreatedDate = o.CreatedOn
        }).OrderByDescending(t => t.CreatedDate).ToList();

        return View(model);
    }

    public async Task<IActionResult> Details(int id)
    {
        var order = await _transferOrderService.GetByIdAsync(id);
        if (order == null)
        {
            return NotFound();
        }

        var model = new TransferOrderModel
        {
            Id = order.Id,
            TransferId = $"TO-{order.CreatedOn.Year}-{order.Id:D3}",
            SKU = order.TransferOrderItems.FirstOrDefault()?.Product.Code ?? "N/A",
            Quantity = order.TransferOrderItems.FirstOrDefault()?.Qty ?? 0,
            FromLocation = order.SourceWarehouse?.Name ?? "Unknown",
            ToLocation = order.DestinationWarehouse?.Name ?? "Unknown",
            Status = order.Status.ToString().ToLower(),
            CreatedDate = order.CreatedOn
        };

        ViewBag.ProductName = order.TransferOrderItems.FirstOrDefault()?.Product.Name;

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create(int productId, int quantity, int fromWarehouseId, int toWarehouseId)
    {
        var newOrder = new TransferOrder
        {
            SourceWarehouseId = fromWarehouseId,
            DestinationWarehouseId = toWarehouseId,
            Status = TransferOrderStatus.Pending,
            TransferOrderItems = new List<TransferOrderItem>
            {
                new TransferOrderItem
                {
                    ProductId = productId,
                    Qty = quantity
                }
            }
        };

        await _transferOrderService.CreateAsync(newOrder);
        TempData["Success"] = "Transfer Order created successfully!";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Approve(int id)
    {
        await _transferOrderService.ApproveAsync(id);
        TempData["Success"] = "Transfer Order approved successfully.";
        return RedirectToAction(nameof(Index));
    }
}

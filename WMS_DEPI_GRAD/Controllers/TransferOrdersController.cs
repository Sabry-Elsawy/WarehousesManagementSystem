using Microsoft.AspNetCore.Mvc;
using WMS_DEPI_GRAD.Models;

namespace WMS_DEPI_GRAD.Controllers;
public class TransferOrdersController : Controller
{
    private static List<TransferOrderModel> _transferOrders = new()
    {
        new TransferOrderModel { Id = 1, TransferId = "TO-2024-001", SKU = "SKU-1001", Quantity = 50, FromLocation = "Main Warehouse", ToLocation = "North Branch", Status = "completed", CreatedDate = new DateTime(2024, 1, 14) },
        new TransferOrderModel { Id = 2, TransferId = "TO-2024-002", SKU = "SKU-1003", Quantity = 25, FromLocation = "South Branch", ToLocation = "Main Warehouse", Status = "in-progress", CreatedDate = new DateTime(2024, 1, 15) },
        new TransferOrderModel { Id = 3, TransferId = "TO-2024-003", SKU = "SKU-1002", Quantity = 100, FromLocation = "East Distribution", ToLocation = "West Hub", Status = "pending", CreatedDate = new DateTime(2024, 1, 16) }
    };

    private readonly WMS.BLL.Interfaces.IWarehouseService _warehouseService;
    private readonly WMS.BLL.Interfaces.IProductService _productService;

    public TransferOrdersController(WMS.BLL.Interfaces.IWarehouseService warehouseService, WMS.BLL.Interfaces.IProductService productService)
    {
        _warehouseService = warehouseService;
        _productService = productService;
    }

    public async Task<IActionResult> Index()
    {
        var warehouses = await _warehouseService.GetAllWarehousesAsync();
        var products = await _productService.GetAllAsync();
        ViewBag.Warehouses = warehouses;
        ViewBag.Products = products;
        return View(_transferOrders.OrderByDescending(t => t.CreatedDate).ToList());
    }

    public async Task<IActionResult> Details(int id)
    {
        var order = _transferOrders.FirstOrDefault(t => t.Id == id);
        if (order == null)
        {
            return NotFound();
        }

        var products = await _productService.GetAllAsync();
        var product = products.FirstOrDefault(p => p.Code == order.SKU);
        ViewBag.ProductName = product?.Name;

        return View(order);
    }

    [HttpPost]
    public IActionResult Create(string sku, int quantity, string fromLocation, string toLocation)
    {
        var newOrder = new TransferOrderModel
        {
            Id = _transferOrders.Max(t => t.Id) + 1,
            TransferId = $"TO-{DateTime.UtcNow.Year}-{_transferOrders.Max(t => t.Id) + 1:D3}",
            SKU = sku,
            Quantity = quantity,
            FromLocation = fromLocation,
            ToLocation = toLocation,
            Status = "pending",
            CreatedDate = DateTime.UtcNow
        };

        _transferOrders.Add(newOrder);
        TempData["Success"] = $"Transfer Order {newOrder.TransferId} created successfully!";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public IActionResult Approve(int id)
    {
        var order = _transferOrders.FirstOrDefault(t => t.Id == id);
        if (order == null)
        {
            return NotFound();
        }

        if (order.Status == "pending")
        {
            order.Status = "approved"; // Or "agreed" based on user terminology, sticking to "approved" for now as per plan
            TempData["Success"] = $"Transfer Order {order.TransferId} has been approved.";
        }

        return RedirectToAction(nameof(Index));
    }
}

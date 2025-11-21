using Microsoft.AspNetCore.Mvc;
using WMS_DEPI_GRAD.Models;

namespace WMS_DEPI_GRAD.Controllers;
public class TransferOrdersController : Controller
{
    private static List<TransferOrderModel> _transferOrders = new()
    {
        new TransferOrderModel { Id = 1, TransferId = "TO-2024-001", SKU = "SKU-1001", Quantity = 50, FromLocation = "A-01-02-03", ToLocation = "B-02-03-01", Status = "completed", CreatedDate = new DateTime(2024, 1, 14) },
        new TransferOrderModel { Id = 2, TransferId = "TO-2024-002", SKU = "SKU-1003", Quantity = 25, FromLocation = "C-01-01-04", ToLocation = "A-03-02-01", Status = "in-progress", CreatedDate = new DateTime(2024, 1, 15) },
        new TransferOrderModel { Id = 3, TransferId = "TO-2024-003", SKU = "SKU-1002", Quantity = 100, FromLocation = "B-02-01-02", ToLocation = "C-01-04-03", Status = "pending", CreatedDate = new DateTime(2024, 1, 16) }
    };

    public IActionResult Index()
    {
        return View(_transferOrders.OrderByDescending(t => t.CreatedDate).ToList());
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
}

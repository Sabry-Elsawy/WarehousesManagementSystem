using Microsoft.AspNetCore.Mvc;
using WMS.BLL.Interfaces;
using WMS_DEPI_GRAD.ViewModels;

namespace WMS_DEPI_GRAD.Controllers;

//[Authorize]
public class ShippingController : Controller
{
    private readonly IShippingService _shippingService;
    private readonly ISalesOrderService _soService;
    private readonly IProductService _productService;

    public ShippingController(
        IShippingService shippingService,
        ISalesOrderService soService,
        IProductService productService)
    {
        _shippingService = shippingService;
        _soService = soService;
        _productService = productService;
    }

    public async Task<IActionResult> Index()
    {
        var ordersReadyForShipping = await _shippingService.GetOrdersReadyForShippingAsync();

        var viewModels = ordersReadyForShipping.Select(so => new ShippingViewModel
        {
            SalesOrderId = so.Id,
            SO_Number = so.SO_Number,
            CustomerName = so.Customer?.Name ?? "",
            WarehouseName = so.Warehouse?.Name ?? "",
            OrderDate = so.OrderDate,
            ItemCount = so.SO_Items?.Count ?? 0,
            TotalQuantity = so.SO_Items?.Sum(i => i.QtyOrdered) ?? 0
        }).ToList();

        return View(viewModels);
    }

    public async Task<IActionResult> Confirm(int id)
    {
        var so = await _soService.GetByIdAsync(id);
        if (so == null)
            return NotFound();

        var products = await _productService.GetAllAsync();

        // Generate delivery note number (preview)
        var deliveryNoteNumber = $"DN-{so.SO_Number}-{DateTime.UtcNow:yyyyMMddHHmmss}";

        var viewModel = new ShippingConfirmationViewModel
        {
            SalesOrderId = so.Id,
            SO_Number = so.SO_Number,
            CustomerName = so.Customer?.Name ?? "",
            WarehouseName = so.Warehouse?.Name ?? "",
            OrderDate = so.OrderDate,
            DeliveryNoteNumber = deliveryNoteNumber,
            Items = so.SO_Items?.Select(i => new ShippingItemViewModel
            {
                ProductName = products.FirstOrDefault(p => p.Id == i.ProductId)?.Name ?? "",
                SKU = products.FirstOrDefault(p => p.Id == i.ProductId)?.Code ?? "",
                QtyOrdered = i.QtyOrdered,
                QtyPicked = i.QtyPicked
            }).ToList() ?? new List<ShippingItemViewModel>()
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Confirm(int id, ShippingConfirmationViewModel viewModel)
    {
        try
        {
            var deliveryNoteNumber = await _shippingService.ShipOrderAsync(id);
            TempData["Success"] = $"Order shipped successfully! Delivery Note: {deliveryNoteNumber}";
            TempData["DeliveryNoteNumber"] = deliveryNoteNumber;
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
            return RedirectToAction(nameof(Confirm), new { id });
        }
    }
}

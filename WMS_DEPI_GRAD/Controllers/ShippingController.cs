using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WMS.BLL.DTOs;
using WMS.BLL.Interfaces;
using WMS.DAL;
using WMS.DAL.Entities._Identity;
using WMS_DEPI_GRAD.ViewModels;

namespace WMS_DEPI_GRAD.Controllers;

//[Authorize]
public class ShippingController : Controller
{
    private readonly ISalesOrderService _soService;
    private readonly IProductService _productService;
    private readonly UserManager<ApplicationUser> _userManager;

    public ShippingController(
        ISalesOrderService soService,
        IProductService productService,
        UserManager<ApplicationUser> userManager)
    {
        _soService = soService;
        _productService = productService;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var allOrders = await _soService.GetAllAsync();
        var ordersReadyForShipping = allOrders
            .Where(so => so.Status == WMS.DAL.SalesOrderStatus.Picked || 
                         so.Status == WMS.DAL.SalesOrderStatus.PartiallyPicked)
            .OrderBy(so => so.OrderDate)
            .ToList();

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

        var deliveryNoteNumber = $"DN-{so.SO_Number}-{DateTime.UtcNow:yyyyMMdd}";

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
    public async Task<IActionResult> Confirm(int id, string carrier, string trackingNumber, string notes)
    {
        try
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var performedBy = currentUser != null ? $"{currentUser.FirstName} {currentUser.LastName}" : User.Identity?.Name ?? "System";

            var shipmentDto = new ShipmentDto
            {
                Carrier = carrier,
                TrackingNumber = trackingNumber,
                Notes = notes ?? string.Empty,
                PerformedBy = performedBy
            };

            await _soService.ShipOrderAsync(id, shipmentDto);
            
            TempData["Success"] = $"Order {id} shipped successfully! Tracking: {trackingNumber}";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
            return RedirectToAction(nameof(Confirm), new { id });
        }
    }
}

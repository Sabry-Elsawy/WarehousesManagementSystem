using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WMS.BLL.DTOs;
using WMS.BLL.Interfaces;
using WMS.DAL;
using WMS.DAL.Entities._Identity;
using WMS.DAL.UnitOfWork;
using WMS_DEPI_GRAD.ViewModels;

namespace WMS_DEPI_GRAD.Controllers;

[Authorize(Roles = "Admin,SalesManager,Storekeeper")]
public class SalesOrderController : Controller
{
    private readonly ISalesOrderService _soService;
    private readonly IPickingService _pickingService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IProductService _productService;
    private readonly UserManager<ApplicationUser> _userManager;

    public SalesOrderController(
        ISalesOrderService soService,
        IPickingService pickingService,
        IUnitOfWork unitOfWork,
        IProductService productService,
        UserManager<ApplicationUser> userManager)
    {
        _soService = soService;
        _pickingService = pickingService;
        _unitOfWork = unitOfWork;
        _productService = productService;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var salesOrders = await _soService.GetAllAsync();

        // Get all unique user IDs
        var userIds = salesOrders.Select(s => s.CreatedBy)
            .Union(salesOrders.Select(s => s.LastModifiedBy))
            .Where(id => !string.IsNullOrEmpty(id))
            .Distinct()
            .ToList();

        // Fetch users dictionary for fast lookup
        var users = new Dictionary<string, string>();
        foreach (var userId in userIds)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                users[userId] = $"{user.FirstName} {user.LastName}";
            }
        }

        var viewModels = salesOrders.Select(so => new SalesOrderViewModel
        {
            Id = so.Id,
            SO_Number = so.SO_Number,
            CustomerId = so.CustomerId,
            CustomerName = so.Customer?.Name ?? "N/A",
            WarehouseId = so.WarehouseId,
            WarehouseName = so.Warehouse?.Name ?? "N/A",
            OrderDate = so.OrderDate,
            Status = so.Status,
            CreatedOn = so.CreatedOn,
            CreatedBy = !string.IsNullOrEmpty(so.CreatedBy) && users.ContainsKey(so.CreatedBy) ? users[so.CreatedBy] : so.CreatedBy,
            LastModifiedOn = so.LastModifiedOn,
            LastModifiedBy = !string.IsNullOrEmpty(so.LastModifiedBy) && users.ContainsKey(so.LastModifiedBy) ? users[so.LastModifiedBy] : so.LastModifiedBy
        }).ToList();

        return View(viewModels);
    }

    public async Task<IActionResult> Details(int id)
    {
        var so = await _soService.GetByIdAsync(id);
        if (so == null)
            return NotFound();

        await LoadLookups();

        var products = await _productService.GetAllAsync();

        var model = new SalesOrderViewModel
        {
            Id = so.Id,
            SO_Number = so.SO_Number,
            CustomerId = so.CustomerId,
            CustomerName = so.Customer?.Name ?? "",
            WarehouseId = so.WarehouseId,
            WarehouseName = so.Warehouse?.Name ?? "",
            OrderDate = so.OrderDate,
            Status = so.Status,
            CreatedOn = so.CreatedOn,
            CreatedBy = so.CreatedBy,
            LastModifiedOn = so.LastModifiedOn,
            LastModifiedBy = so.LastModifiedBy,
            Items = so.SO_Items?.Select(i => new SalesOrderItemViewModel
            {
                Id = i.Id,
                ProductId = i.ProductId,
                ProductName = products.FirstOrDefault(p => p.Id == i.ProductId)?.Name ?? "",
                SKU = products.FirstOrDefault(p => p.Id == i.ProductId)?.Code ?? "",
                QtyOrdered = i.QtyOrdered,
                QtyPicked = i.QtyPicked,
                UnitPrice = i.UnitPrice
            }).ToList() ?? new List<SalesOrderItemViewModel>()
        };

        return View(model);
    }

    public async Task<IActionResult> Create()
    {
        await LoadLookups();
        return View(new CreateSalesOrderViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateSalesOrderViewModel viewModel)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var salesOrder = new SalesOrder
                {
                    SO_Number = viewModel.SO_Number,
                    CustomerId = viewModel.CustomerId,
                    WarehouseId = viewModel.WarehouseId,
                    OrderDate = viewModel.OrderDate,
                    Status = SalesOrderStatus.Draft
                };

                await _soService.CreateAsync(salesOrder);
                TempData["Success"] = "Sales Order created successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
        }

        await LoadLookups();
        return View(viewModel);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var so = await _soService.GetByIdAsync(id);
        if (so == null)
            return NotFound();

        var viewModel = new SalesOrderViewModel
        {
            Id = so.Id,
            SO_Number = so.SO_Number,
            CustomerId = so.CustomerId,
            CustomerName = so.Customer?.Name ?? "",
            WarehouseId = so.WarehouseId,
            WarehouseName = so.Warehouse?.Name ?? "",
            OrderDate = so.OrderDate,
            Status = so.Status,
            CreatedOn = so.CreatedOn,
            CreatedBy = so.CreatedBy,
            LastModifiedOn = so.LastModifiedOn,
            LastModifiedBy = so.LastModifiedBy
        };

        await LoadLookups();
        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, SalesOrderViewModel viewModel)
    {
        if (id != viewModel.Id)
            return BadRequest();

        if (ModelState.IsValid)
        {
            try
            {
                var salesOrder = await _soService.GetByIdAsync(id);
                if (salesOrder == null)
                {
                    TempData["Error"] = "Sales Order not found.";
                    return NotFound();
                }

                salesOrder.SO_Number = viewModel.SO_Number;
                salesOrder.CustomerId = viewModel.CustomerId;
                salesOrder.WarehouseId = viewModel.WarehouseId;
                salesOrder.OrderDate = viewModel.OrderDate;

                await _soService.UpdateAsync(salesOrder);
                TempData["Success"] = "Sales Order updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while updating the Sales Order.");
            }
        }

        await LoadLookups();
        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Submit(int id)
    {
        try
        {
            await _soService.SubmitAsync(id);

            // Auto-allocate picking tasks
            await _pickingService.AllocatePickingTasksAsync(id);

            TempData["Success"] = "Sales Order submitted successfully! Picking tasks have been allocated.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    public async Task<IActionResult> AddItem(int salesOrderId, int productId, int qtyOrdered, decimal unitPrice)
    {
        try
        {
            var product = await _productService.GetByIdAsync(productId);
            if (product == null)
            {
                TempData["Error"] = "Product not found.";
                return RedirectToAction(nameof(Details), new { id = salesOrderId });
            }

            var item = new SO_Item
            {
                ProductId = productId,
                QtyOrdered = qtyOrdered,
                UnitPrice = unitPrice
            };

            await _soService.AddItemAsync(salesOrderId, item);
            TempData["Success"] = "Item added successfully!";
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Details), new { id = salesOrderId });
    }

    [HttpPost]
    public async Task<IActionResult> RemoveItem(int salesOrderId, int itemId)
    {
        try
        {
            await _soService.RemoveItemAsync(itemId);
            TempData["Success"] = "Item removed successfully!";
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Details), new { id = salesOrderId });
    }

    [HttpPost]
    public async Task<IActionResult> ShipOrder(int id, string carrier, string trackingNumber, string notes)
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
            TempData["Success"] = $"Sales Order shipped successfully! Tracking: {trackingNumber}";
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    public async Task<IActionResult> CancelOrder(int id, string reason)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(reason) || reason.Length < 10)
            {
                TempData["Error"] = "Cancellation reason must be at least 10 characters.";
                return RedirectToAction(nameof(Details), new { id });
            }

            await _soService.CancelSalesOrderAsync(id, reason);
            TempData["Success"] = "Sales Order cancelled successfully.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _soService.DeleteAsync(id);
            TempData["Success"] = "Sales Order deleted successfully!";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
            return RedirectToAction(nameof(Details), new { id });
        }
    }

    private async Task LoadLookups()
    {
        var customerRepo = _unitOfWork.GetRepository<Customer, int>();
        var warehouseRepo = _unitOfWork.GetRepository<Warehouse, int>();

        ViewBag.Customers = new SelectList(await customerRepo.GetAllAsync(), "Id", "Name");
        ViewBag.Warehouses = new SelectList(await warehouseRepo.GetAllAsync(), "Id", "Name");

        var products = await _productService.GetAllAsync();
        ViewBag.Products = products
            .Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = $"{p.Code} - {p.Name}"
            })
            .ToList();
    }
}

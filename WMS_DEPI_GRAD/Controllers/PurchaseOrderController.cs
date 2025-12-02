using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WMS.BLL.Interfaces;
using WMS.BLL.Services;
using WMS.DAL;
using WMS.DAL.UnitOfWork;

namespace WMS_DEPI_GRAD.Controllers;

//[Authorize]
public class PurchaseOrderController : Controller
{
    private readonly IPurchaseOrderService _poService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IProductService _productService;

    public PurchaseOrderController(IPurchaseOrderService poService, IUnitOfWork unitOfWork, IProductService productService)
    {
        _poService = poService;
        _unitOfWork = unitOfWork;
        _productService = productService;
    }

    public async Task<IActionResult> Index()
    {
        var purchaseOrders = await _poService.GetAllAsync();
        var viewModels = purchaseOrders.Select(po => new PurchaseOrderViewModel
        {
            Id = po.Id,
            PO_Number = po.PO_Number,
            VendorId = po.VendorId,
            VendorName = po.Vendor?.Name ?? "",
            WarehouseId = po.WarehouseId,
            WarehouseName = po.Warehouse?.Name ?? "",
            OrderDate = po.OrderDate,
            ExpectedArrivalDate = po.ExpectedArrivalDate,
            Status = po.Status,
            CreatedOn = po.CreatedOn,
            CreatedBy = po.CreatedBy,
            LastModifiedOn = po.LastModifiedOn,
            LastModifiedBy = po.LastModifiedBy
        }).ToList();

        return View(viewModels);
    }

    //public async Task<IActionResult> Details(int id)
    //{
    //    var po = await _poService.GetByIdAsync(id);
    //    if (po == null)
    //        return NotFound();

    //    var viewModel = new PurchaseOrderViewModel
    //    {
    //        Id = po.Id,
    //        PO_Number = po.PO_Number,
    //        VendorId = po.VendorId,
    //        VendorName = po.Vendor?.Name ?? "",
    //        WarehouseId = po.WarehouseId,
    //        WarehouseName = po.Warehouse?.Name ?? "",
    //        OrderDate = po.OrderDate,
    //        ExpectedArrivalDate = po.ExpectedArrivalDate,
    //        Status = po.Status,
    //        CreatedOn = po.CreatedOn,
    //        CreatedBy = po.CreatedBy,
    //        LastModifiedOn = po.LastModifiedOn,
    //        LastModifiedBy = po.LastModifiedBy,
    //        Items = po.POItems.Select(i => new PurchaseOrderItemViewModel
    //        {
    //            Id = i.Id,
    //            ProductId = i.ProductId,
    //            ProductName = i.Product?.Name ?? "",
    //            SKU = i.Product?.Code ?? "", // Assuming Code is SKU
    //            QtyOrdered = i.QtyOrdered,
    //            UnitPrice = i.UnitPrice,
    //            QtyReceived = i.QtyReceived,
    //            LineStatus = i.QtyReceived >= i.QtyOrdered ? "Complete" : (i.QtyReceived > 0 ? "Partial" : "Open")
    //        }).ToList()
    //    };

    //    return View(viewModel);
    //}


    public async Task<IActionResult> Details(int id)
    {
        var po = await _poService.GetByIdAsync(id);
        if (po == null)
            return NotFound();

        await LoadLookups();

        // We need products list for mapping names and SKUs in the items list
        // LoadLookups puts them in ViewBag as SelectListItems, which might not have SKU easily accessible if we only put "Code - Name" in Text.
        // So let's fetch products again or use the service.
        var products = await _productService.GetAllAsync();

        var model = new PurchaseOrderViewModel
        {
            Id = po.Id,
            PO_Number = po.PO_Number,
            VendorId = po.VendorId,
            VendorName = po.Vendor?.Name ?? "",
            WarehouseId = po.WarehouseId,
            WarehouseName = po.Warehouse?.Name ?? "",
            OrderDate = po.OrderDate,
            ExpectedArrivalDate = po.ExpectedArrivalDate,
            Status = po.Status,
            CreatedOn = po.CreatedOn,
            CreatedBy = po.CreatedBy,
            LastModifiedOn = po.LastModifiedOn,
            LastModifiedBy = po.LastModifiedBy,
            Items = po.POItems?.Select(i => new PurchaseOrderItemViewModel
            {
                ProductName = products.FirstOrDefault(p => p.Id == i.ProductId)?.Name ?? "",
                SKU = products.FirstOrDefault(p => p.Id == i.ProductId)?.Code ?? "",
                QtyOrdered = i.QtyOrdered,
                QtyReceived = i.QtyReceived,
                UnitPrice = i.UnitPrice
            }).ToList() ?? new List<PurchaseOrderItemViewModel>()
        };

        return View(model);
    }

    public async Task<IActionResult> Create()
    {
        await LoadLookups();
        return View(new CreatePurchaseOrderViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    //[Authorize(Roles = "Procurement,Admin")]
    public async Task<IActionResult> Create(CreatePurchaseOrderViewModel viewModel)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var purchaseOrder = new PurchaseOrder
                {
                    PO_Number = viewModel.PO_Number,
                    VendorId = viewModel.VendorId,
                    WarehouseId = viewModel.WarehouseId,
                    ExpectedArrivalDate = viewModel.ExpectedArrivalDate,
                    OrderDate = DateTime.UtcNow,
                    Status = PurchaseOrderStatus.Open
                };

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
        return View(viewModel);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var po = await _poService.GetByIdAsync(id);
        if (po == null)
            return NotFound();

        var viewModel = new PurchaseOrderViewModel
        {
            Id = po.Id,
            PO_Number = po.PO_Number,
            VendorId = po.VendorId,
            VendorName = po.Vendor?.Name ?? "",
            WarehouseId = po.WarehouseId,
            WarehouseName = po.Warehouse?.Name ?? "",
            OrderDate = po.OrderDate,
            ExpectedArrivalDate = po.ExpectedArrivalDate,
            Status = po.Status,
            CreatedOn = po.CreatedOn,
            CreatedBy = po.CreatedBy,
            LastModifiedOn = po.LastModifiedOn,
            LastModifiedBy = po.LastModifiedBy
        };

        await LoadLookups();
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    //[Authorize(Roles = "Procurement,Admin")]
    public async Task<IActionResult> Edit(int id, PurchaseOrderViewModel viewModel)
    {
        if (id != viewModel.Id)
            return BadRequest();

        if (ModelState.IsValid)
        {
            try
            {
                // Get the existing PO entity from database
                var purchaseOrder = await _poService.GetByIdAsync(id);
                if (purchaseOrder == null)
                {
                    TempData["Error"] = "Purchase Order not found.";
                    return NotFound();
                }

                // Only update editable fields - do NOT touch:
                // - Status (updated via Approve/Close actions)
                // - CreatedOn, CreatedBy (set at creation)
                // - LastModifiedOn, LastModifiedBy (handled by service layer)
                // - POItems collection (managed via AddItem action)
                purchaseOrder.PO_Number = viewModel.PO_Number;
                purchaseOrder.VendorId = viewModel.VendorId;
                purchaseOrder.WarehouseId = viewModel.WarehouseId;
                purchaseOrder.ExpectedArrivalDate = viewModel.ExpectedArrivalDate;

                await _poService.UpdateAsync(purchaseOrder);
                TempData["Success"] = "Purchase Order updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while updating the Purchase Order.");
                // Log the exception details here if you have a logging framework
            }
        }

        await LoadLookups();
        return View(viewModel);
    }

    [HttpPost]
    //[Authorize(Roles = "Procurement,Admin")]
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
    //[Authorize(Roles = "Procurement,Admin")]
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
    //[Authorize(Roles = "Procurement,Admin")]
    public async Task<IActionResult> AddItem([FromBody] ViewModels.AddPurchaseOrderItemRequest request)
    {
        if (request == null || request.Item == null)
        {
            return Json(new { success = false, message = "Invalid request data." });
        }

        try
        {
            // Fetch the product to get the SKU
            var product = await _productService.GetByIdAsync(request.Item.ProductId);
            if (product == null)
            {
                return Json(new { success = false, message = "Product not found." });
            }

            // Create the PO item with all required fields including SKU
            var item = new PurchaseOrderItem
            {
                ProductId = request.Item.ProductId,
                QtyOrdered = request.Item.QtyOrdered,
                UnitPrice = request.Item.UnitPrice,
                SKU = product.Code // Set SKU from Product.Code
            };

            await _poService.AddItemAsync(request.PoId, item);
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

        ViewBag.Vendors = new SelectList(await vendorRepo.GetAllAsync(), "Id", "Name");
        ViewBag.Warehouses = new SelectList(await warehouseRepo.GetAllAsync(), "Id", "Name");

        // ✔️ المنتجـات من ProductService مش من UnitOfWork
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

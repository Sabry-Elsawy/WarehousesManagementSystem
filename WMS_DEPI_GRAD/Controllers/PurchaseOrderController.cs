using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WMS.BLL.Interfaces;
using WMS.DAL;
using WMS.DAL.UnitOfWork;

namespace WMS_DEPI_GRAD.Controllers;

//[Authorize]
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

    public async Task<IActionResult> Details(int id)
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
            LastModifiedBy = po.LastModifiedBy,
            Items = po.POItems.Select(i => new PurchaseOrderItemViewModel
            {
                Id = i.Id,
                ProductId = i.ProductId,
                ProductName = i.Product?.Name ?? "",
                SKU = i.Product?.Code ?? "", // Assuming Code is SKU
                QtyOrdered = i.QtyOrdered,
                UnitPrice = i.UnitPrice,
                QtyReceived = i.QtyReceived,
                LineStatus = i.QtyReceived >= i.QtyOrdered ? "Complete" : (i.QtyReceived > 0 ? "Partial" : "Open")
            }).ToList()
        };

        return View(viewModel);
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
                // We need to get the existing PO to update it, or create a new object with just the updated fields
                // The service likely expects a full object or handles partial updates. 
                // Assuming we map back to a domain object.
                
                var purchaseOrder = await _poService.GetByIdAsync(id);
                if (purchaseOrder == null)
                    return NotFound();

                purchaseOrder.PO_Number = viewModel.PO_Number;
                purchaseOrder.VendorId = viewModel.VendorId;
                purchaseOrder.WarehouseId = viewModel.WarehouseId;
                purchaseOrder.ExpectedArrivalDate = viewModel.ExpectedArrivalDate;
                // Status is usually not updated via Edit, but via transitions

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
        try
        {
            var item = new PurchaseOrderItem
            {
                ProductId = request.Item.ProductId,
                QtyOrdered = request.Item.QtyOrdered,
                UnitPrice = request.Item.UnitPrice
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
        var productRepo = _unitOfWork.GetRepository<Product, int>();

        ViewBag.Vendors = new SelectList(await vendorRepo.GetAllAsync(), "Id", "Name");
        ViewBag.Warehouses = new SelectList(await warehouseRepo.GetAllAsync(), "Id", "Name");
        ViewBag.Products = new SelectList(await productRepo.GetAllAsync(), "Id", "Name");
    }
}

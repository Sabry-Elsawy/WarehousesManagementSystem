using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WMS.BLL.Interfaces;
using WMS.BLL.Services;
using WMS.DAL;
using WMS.DAL.UnitOfWork;

namespace WMS_DEPI_GRAD.Controllers;

//[Authorize]
public class ASNController : Controller
{
    private readonly IASNService _asnService;
    private readonly IPurchaseOrderService _poService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IProductService _productService;

    public ASNController(IASNService asnService, IPurchaseOrderService poService, IUnitOfWork unitOfWork, IProductService productService)
    {
        _asnService = asnService;
        _poService = poService;
        _unitOfWork = unitOfWork;
        _productService = productService;
    }

    public async Task<IActionResult> Index()
    {
        var asns = await _asnService.GetAllAsync();
        var viewModels = asns.Select(asn => new ASNViewModel
        {
            Id = asn.Id,
            ASN_Number = asn.ASN_Number,
            PurchaseOrderId = asn.PurchaseOrderId,
            PO_Number = asn.PurchaseOrder?.PO_Number ?? "",
            ExpectedArrivalDate = asn.ExpectedArrivalDate,
            TrackingNumber = asn.TrackingNumber,
            Status = asn.Status,
            CreatedOn = asn.CreatedOn,
            CreatedBy = asn.CreatedBy
        }).ToList();

        return View(viewModels);
    }

    //public async Task<IActionResult> Details(int id)
    //{
    //    var asn = await _asnService.GetByIdAsync(id);
    //    if (asn == null)
    //        return NotFound();

    //    var viewModel = new ASNViewModel
    //    {
    //        Id = asn.Id,
    //        ASN_Number = asn.ASN_Number,
    //        PurchaseOrderId = asn.PurchaseOrderId,
    //        PO_Number = asn.PurchaseOrder?.PO_Number ?? "",
    //        ExpectedArrivalDate = asn.ExpectedArrivalDate,
    //        TrackingNumber = asn.TrackingNumber,
    //        Status = asn.Status,
    //        CreatedOn = asn.CreatedOn,
    //        CreatedBy = asn.CreatedBy,
    //        Items = asn.ASNItems.Select(i => new ASNItemViewModel
    //        {
    //            Id = i.Id,
    //            ProductId = i.ProductId,
    //            ProductName = i.Product?.Name ?? "",
    //            SKU = i.Product?.Code ?? "",
    //            QtyShipped = i.QtyShipped,
    //            QtyOrdered = i.LinkedPOItemId.HasValue ? asn.PurchaseOrder?.POItems.FirstOrDefault(p => p.Id == i.LinkedPOItemId)?.QtyOrdered : null,
    //            LinkedPOItemId = i.LinkedPOItemId
    //        }).ToList(),
    //        POItems = asn.PurchaseOrder?.POItems.Select(p => new PurchaseOrderItemViewModel
    //        {
    //            Id = p.Id,
    //            ProductId = p.ProductId,
    //            ProductName = p.Product?.Name ?? "",
    //            QtyOrdered = p.QtyOrdered,
    //            QtyReceived = p.QtyReceived
    //        }).ToList() ?? new List<PurchaseOrderItemViewModel>()
    //    };

    //    return View(viewModel);
    //}


    public async Task<IActionResult> Details(int id)
    {
        var asn = await _asnService.GetByIdAsync(id);
        if (asn == null)
            return NotFound();

        var viewModel = new ASNViewModel
        {
            Id = asn.Id,
            ASN_Number = asn.ASN_Number,
            PurchaseOrderId = asn.PurchaseOrderId,
            PO_Number = asn.PurchaseOrder?.PO_Number ?? "",
            ExpectedArrivalDate = asn.ExpectedArrivalDate,
            TrackingNumber = asn.TrackingNumber,
            Status = asn.Status,
            CreatedOn = asn.CreatedOn,
            CreatedBy = asn.CreatedBy,

            Items = asn.ASNItems.Select(i => new ASNItemViewModel
            {
                Id = i.Id,
                ProductId = i.ProductId,
                ProductName = i.Product?.Name ?? "",
                SKU = i.Product?.Code ?? "",
                QtyShipped = i.QtyShipped,
                QtyOrdered = i.LinkedPOItemId.HasValue
                    ? asn.PurchaseOrder?.POItems.FirstOrDefault(p => p.Id == i.LinkedPOItemId)?.QtyOrdered
                    : null,
                LinkedPOItemId = i.LinkedPOItemId
            }).ToList(),

            POItems = asn.PurchaseOrder?.POItems.Select(p => new PurchaseOrderItemViewModel
            {
                Id = p.Id,
                ProductId = p.ProductId,
                ProductName = p.Product?.Name ?? "",
                QtyOrdered = p.QtyOrdered,
                QtyReceived = p.QtyReceived
            }).ToList() ?? new List<PurchaseOrderItemViewModel>()
        };

        var products = await _productService.GetAllAsync();

        ViewBag.Products = products
            .Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = $"{p.Code} - {p.Name}"
            })
            .ToList();

        return View(viewModel);
    }


    //[Authorize(Roles = "Procurement,Admin")]
    public async Task<IActionResult> Create(int? poId)
    {
        await LoadLookups();
        
        var viewModel = new CreateASNViewModel();
        if (poId.HasValue)
            viewModel.PurchaseOrderId = poId.Value;

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    //[Authorize(Roles = "Procurement,Admin")]
    public async Task<IActionResult> Create(CreateASNViewModel viewModel)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var asn = new AdvancedShippingNotice
                {
                    ASN_Number = viewModel.ASN_Number,
                    PurchaseOrderId = viewModel.PurchaseOrderId,
                    TrackingNumber = viewModel.TrackingNumber,
                    ExpectedArrivalDate = viewModel.ExpectedArrivalDate,
                    Status = AdvancedShippingNoticeStatus.Sent
                };

                await _asnService.CreateFromPOAsync(viewModel.PurchaseOrderId, asn);
                TempData["Success"] = "ASN created successfully!";
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

    public async Task<IActionResult> Edit(int id)
    {
        var asn = await _asnService.GetByIdAsync(id);
        if (asn == null)
            return NotFound();

        var viewModel = new ASNViewModel
        {
            Id = asn.Id,
            ASN_Number = asn.ASN_Number,
            PurchaseOrderId = asn.PurchaseOrderId,
            PO_Number = asn.PurchaseOrder?.PO_Number ?? "",
            ExpectedArrivalDate = asn.ExpectedArrivalDate,
            TrackingNumber = asn.TrackingNumber,
            Status = asn.Status,
            CreatedOn = asn.CreatedOn,
            CreatedBy = asn.CreatedBy
        };

        await LoadLookups();
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    //[Authorize(Roles = "Procurement,Admin")]
    public async Task<IActionResult> Edit(int id, ASNViewModel viewModel)
    {
        if (id != viewModel.Id)
            return BadRequest();

        if (ModelState.IsValid)
        {
            try
            {
                var asn = await _asnService.GetByIdAsync(id);
                if (asn == null)
                    return NotFound();

                asn.ASN_Number = viewModel.ASN_Number;
                asn.TrackingNumber = viewModel.TrackingNumber;
                asn.ExpectedArrivalDate = viewModel.ExpectedArrivalDate;
                
                await _asnService.UpdateAsync(asn);
                TempData["Success"] = "ASN updated successfully!";
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
    //[Authorize(Roles = "Warehouse,Admin")]
    public async Task<IActionResult> MarkReceived(int id)
    {
        try
        {
            await _asnService.MarkReceivedAsync(id);
            TempData["Success"] = "ASN marked as received!";
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    //[Authorize(Roles = "Warehouse,Admin")]
    public async Task<IActionResult> Close(int id)
    {
        try
        {
            await _asnService.CloseAsync(id);
            TempData["Success"] = "ASN closed successfully!";
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    //[Authorize(Roles = "Procurement,Admin")]
    public async Task<IActionResult> AddItem([FromBody] ViewModels.AddASNItemRequest request)
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

            // Create the ASN item with all required fields including SKU
            var item = new AdvancedShippingNoticeItem
            {
                ProductId = request.Item.ProductId,
                QtyShipped = request.Item.QtyShipped,
                LinkedPOItemId = request.Item.LinkedPOItemId,
                SKU = product.Code // Set SKU from Product.Code
            };

            await _asnService.AddItemAsync(request.AsnId, item);
            return Json(new { success = true, message = "Item added successfully" });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    private async Task LoadLookups()
    {
        var pos = await _poService.GetAllAsync();
        var approvedPOs = pos.Where(p => p.Status == PurchaseOrderStatus.Approved);
        
        ViewBag.PurchaseOrders = new SelectList(approvedPOs, "Id", "PO_Number");

        var productRepo = _unitOfWork.GetRepository<Product, int>();
        ViewBag.Products = new SelectList(await productRepo.GetAllAsync(), "Id", "Name");
    }
}

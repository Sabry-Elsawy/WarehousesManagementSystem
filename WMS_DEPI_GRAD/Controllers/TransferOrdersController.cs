using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WMS.BLL.Interfaces;
using WMS.DAL;
using WMS_DEPI_GRAD.ViewModels;

namespace WMS_DEPI_GRAD.Controllers
{
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

        public async Task<IActionResult> Index(int page = 1, string search = null)
        {
            var (items, totalCount) = await _transferOrderService.GetPagedListAsync(page, 10, search);

            var viewModels = items.Select(x => new TransferOrderViewModel
            {
                Id = x.Id,
                Status = x.Status.ToString(),
                CreatedOn = x.CreatedOn,
                SourceWarehouse = x.SourceWarehouse?.Name ?? "Unknown",
                DestinationWarehouse = x.DestinationWarehouse?.Name ?? "Unknown",
                ItemCount = x.TransferOrderItems?.Count ?? 0
            }).ToList();

            // Simple pagination handling for now (can be improved with a PagedListViewModel)
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalCount / 10.0);
            ViewBag.SearchTerm = search;

            return View(viewModels);
        }

        public async Task<IActionResult> Details(int id)
        {
            var order = await _transferOrderService.GetByIdAsync(id);
            if (order == null) return NotFound();

            var viewModel = new TransferOrderDetailsViewModel
            {
                Id = order.Id,
                Status = order.Status.ToString(),
                CreatedOn = order.CreatedOn,
                SourceWarehouse = order.SourceWarehouse?.Name ?? "Unknown",
                DestinationWarehouse = order.DestinationWarehouse?.Name ?? "Unknown",
                Items = order.TransferOrderItems.Select(i => new TransferOrderItemDetailsViewModel
                {
                    ProductName = i.Product?.Name ?? "Unknown",
                    SKU = i.Product?.Code ?? "Unknown",
                    Quantity = i.Qty
                }).ToList()
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var warehouses = await _warehouseService.GetAllWarehousesAsync();
            var products = await _productService.GetAllAsync();

            ViewBag.Warehouses = new SelectList(warehouses, "Id", "Name");
            ViewBag.Products = products.Select(p => new { p.Id, Name = $"{p.Name} ({p.Code})" }).ToList();

            return View(new CreateTransferOrderViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateTransferOrderViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var transferOrder = new TransferOrder
                    {
                        SourceWarehouseId = model.SourceWarehouseId,
                        DestinationWarehouseId = model.DestinationWarehouseId,
                        TransferOrderItems = model.Items.Select(i => new TransferOrderItem
                        {
                            ProductId = i.ProductId,
                            Qty = i.Quantity
                        }).ToList()
                    };

                    await _transferOrderService.CreateAsync(transferOrder);
                    TempData["Success"] = "Transfer Order created successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }

            // Reload dropdowns if failed
            var warehouses = await _warehouseService.GetAllWarehousesAsync();
            var products = await _productService.GetAllAsync();
            ViewBag.Warehouses = new SelectList(warehouses, "Id", "Name");
            ViewBag.Products = products.Select(p => new { p.Id, Name = $"{p.Name} ({p.Code})" }).ToList();

            return View(model);
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WMS.BLL.DTOs;
using WMS.BLL.Interfaces;

namespace WMS_DEPI_GRAD.Controllers
{
    public class InventoryController : Controller
    {
        private readonly IInventoryService _inventoryService;
        private readonly IBinService _binService;
        private readonly IProductService _productService;

        public InventoryController(IInventoryService inventoryService, IBinService binService, IProductService productService)
        {
            _inventoryService = inventoryService;
            _binService = binService;
            _productService = productService;
        }

        public async Task<IActionResult> Index(string? search, int? productId, int? binId, string? status, int page = 1, int pageSize = 20)
        {
            var (items, totalCount) = await _inventoryService.GetPagedAsync(page, pageSize, search, productId, binId, status);
            
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalCount = totalCount;
            ViewBag.TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            ViewBag.Search = search;
            ViewBag.ProductId = productId;
            ViewBag.BinId = binId;
            ViewBag.Status = status;

            // Get stock summary for dashboard cards
            var summary = await _inventoryService.GetStockSummaryAsync();
            ViewBag.StockSummary = summary;

            // Load products and bins for Add SKU dropdown
            await LoadDropdownsAsync();

            return View(items);
        }

        private async Task LoadDropdownsAsync()
        {
            // Get all products for dropdown
            var products = await _productService.GetAllAsync();
            ViewBag.Products = new SelectList(products, "Id", "Name");
            
            // Get all bins for dropdown  
            var bins = await _binService.GetAllBinsAsync();
            ViewBag.Bins = new SelectList(bins, "Code", "Code");
        }

        public async Task<IActionResult> Details(int id)
        {
            var inventory = await _inventoryService.GetByIdAsync(id);
            if (inventory == null)
                return NotFound();

            return View(inventory);
        }

        public async Task<IActionResult> StockByProduct(int productId)
        {
            var items = await _inventoryService.GetByProductIdAsync(productId);
            return View("Index", items);
        }

        public async Task<IActionResult> StockByBin(int binId)
        {
            var items = await _inventoryService.GetByBinIdAsync(binId);
            return View("Index", items);
        }

        [HttpPost]
        public async Task<IActionResult> AddInventory(AddInventoryDto model)
        {
            if (!ModelState.IsValid)
                return RedirectToAction("Index");

            try
            {
                // Set performed by from current user
                model.PerformedBy = User.Identity?.Name ?? "System";
                
                await _inventoryService.AddInventoryAsync(model);
                TempData["Success"] = "SKU added successfully!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> AdjustInventory(AdjustInventoryDto model)
        {
            if (!ModelState.IsValid)
                return RedirectToAction("Index");

            try
            {
                // Set performed by from current user
                model.PerformedBy = User.Identity?.Name ?? "System";
                
                await _inventoryService.AdjustInventoryAsync(model);
                TempData["Success"] = "Inventory adjusted successfully!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Transfer(int id)
        {
            var inventory = await _inventoryService.GetByIdAsync(id);
            if (inventory == null)
                return NotFound();

            var bins = await _binService.GetAllBinsAsync();
            ViewBag.Bins = bins;

            return View(inventory);
        }

        [HttpPost]
        public async Task<IActionResult> Transfer(TransferInventoryDto model)
        {
            if (!ModelState.IsValid)
            {
                var inventory = await _inventoryService.GetByIdAsync(model.InventoryId);
                return View(inventory);
            }

            try
            {
                // Set performed by from current user (you might get this from authentication)
                model.PerformedBy = User.Identity?.Name ?? "System";
                
                await _inventoryService.TransferInventoryAsync(model);
                TempData["Success"] = "Inventory transferred successfully!";
                return RedirectToAction("Details", new { id = model.InventoryId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                var inventory = await _inventoryService.GetByIdAsync(model.InventoryId);
                return View(inventory);
            }
        }
        public async Task<IActionResult> Transactions(int? productId, int? binId)
        {
            var transactions = await _inventoryService.GetTransactionsAsync(productId, binId);
            ViewBag.ProductId = productId;
            ViewBag.BinId = binId;
            return View(transactions);
        }
    }
}

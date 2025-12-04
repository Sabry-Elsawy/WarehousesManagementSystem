using Microsoft.AspNetCore.Mvc;
using WMS.BLL.DTOs;
using WMS.BLL.Interfaces;

namespace WMS_DEPI_GRAD.Controllers
{
    public class InventoryController(IInventoryService inventoryService) : Controller
    {
        private readonly IInventoryService _inventoryService = inventoryService;

        public async Task<IActionResult> Index(string? search)
        {
            var items = await _inventoryService.GetAllAsync(search);
            return View(items);
        }

        [HttpPost]
        public async Task<IActionResult> AddInventory(AddInventoryDto model)
        {
            if (!ModelState.IsValid)
                return RedirectToAction("Index");

            try
            {
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
                await _inventoryService.AdjustInventoryAsync(model);
                TempData["Success"] = "Inventory adjusted successfully!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction("Index");
        }


    }
}

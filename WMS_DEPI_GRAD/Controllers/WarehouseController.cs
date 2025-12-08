using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WMS.BLL.Interfaces;
using WMS.DAL;

namespace WMS_DEPI_GRAD.Controllers
{
    [Authorize]
    public class WarehouseController : Controller
    {
        private readonly IWarehouseService _warehouseService;

        public WarehouseController(IWarehouseService warehouseService)
        {
            _warehouseService = warehouseService;
        }

        public async Task<IActionResult> Index()
        {
            var warehouses = await _warehouseService.GetAllWarehousesAsync();
            return View(warehouses);
        }

        public async Task<IActionResult> Details(int id)
        {
            var warehouse = await _warehouseService.GetWarehouseByIdAsync(id);
            if (warehouse == null)
            {
                return NotFound();
            }
            return View(warehouse);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(Warehouse warehouse)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _warehouseService.CreateWarehouseAsync(warehouse);
                    TempData["Success"] = "Warehouse created successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (ArgumentException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            return View(warehouse);
        }

        [Authorize(Roles = "Admin,WarehouseManager")]
        public async Task<IActionResult> Edit(int id)
        {
            var warehouse = await _warehouseService.GetWarehouseByIdAsync(id);
            if (warehouse == null)
            {
                return NotFound();
            }
            return View(warehouse);
        }

        [Authorize(Roles = "Admin,WarehouseManager")]
        [HttpPost]
        public async Task<IActionResult> Edit(int id, Warehouse warehouse)
        {
            if (id != warehouse.Id)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _warehouseService.UpdateWarehouseAsync(warehouse);
                    TempData["Success"] = "Warehouse updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            return View(warehouse);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var warehouse = await _warehouseService.GetWarehouseByIdAsync(id);
            if (warehouse == null)
            {
                return NotFound();
            }
            return View(warehouse);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _warehouseService.DeleteWarehouseAsync(id);
                TempData["Success"] = "Warehouse deleted successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }
    }
}

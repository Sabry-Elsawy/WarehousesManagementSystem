using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WMS.BLL.Interfaces;
using WMS.DAL;

namespace WMS_DEPI_GRAD.Controllers
{
    [Authorize]
    public class ZoneController : Controller
    {
        private readonly IZoneService _zoneService;
        private readonly IWarehouseService _warehouseService;

        public ZoneController(IZoneService zoneService, IWarehouseService warehouseService)
        {
            _zoneService = zoneService;
            _warehouseService = warehouseService;
        }

        public async Task<IActionResult> Index()
        {
            var zones = await _zoneService.GetAllZonesAsync();
            return View(zones);
        }

        public async Task<IActionResult> Details(int id)
        {
            var zone = await _zoneService.GetZoneByIdAsync(id);
            if (zone == null)
            {
                return NotFound();
            }
            return View(zone);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            await PopulateWarehousesDropdown();
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(Zone zone)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _zoneService.CreateZoneAsync(zone);
                    TempData["Success"] = "Zone created successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (ArgumentException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            await PopulateWarehousesDropdown();
            return View(zone);
        }

        [Authorize(Roles = "Admin,WarehouseManager")]
        public async Task<IActionResult> Edit(int id)
        {
            var zone = await _zoneService.GetZoneByIdAsync(id);
            if (zone == null)
            {
                return NotFound();
            }
            await PopulateWarehousesDropdown();
            return View(zone);
        }

        [Authorize(Roles = "Admin,WarehouseManager")]
        [HttpPost]
        public async Task<IActionResult> Edit(int id, Zone zone)
        {
            if (id != zone.Id)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _zoneService.UpdateZoneAsync(zone);
                    TempData["Success"] = "Zone updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            await PopulateWarehousesDropdown();
            return View(zone);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var zone = await _zoneService.GetZoneByIdAsync(id);
            if (zone == null)
            {
                return NotFound();
            }
            return View(zone);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _zoneService.DeleteZoneAsync(id);
                TempData["Success"] = "Zone deleted successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        private async Task PopulateWarehousesDropdown()
        {
            var warehouses = await _warehouseService.GetAllWarehousesAsync();
            ViewBag.Warehouses = new SelectList(warehouses, "Id", "Name");
        }
    }
}

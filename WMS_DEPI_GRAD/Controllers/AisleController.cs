using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WMS.BLL.Interfaces;
using WMS.DAL;

namespace WMS_DEPI_GRAD.Controllers
{
    public class AisleController : Controller
    {
        private readonly IAisleService _aisleService;
        private readonly IZoneService _zoneService;

        public AisleController(IAisleService aisleService, IZoneService zoneService)
        {
            _aisleService = aisleService;
            _zoneService = zoneService;
        }

        public async Task<IActionResult> Index()
        {
            var aisles = await _aisleService.GetAllAislesAsync();
            return View(aisles);
        }

        public async Task<IActionResult> Details(int id)
        {
            var aisle = await _aisleService.GetAisleByIdAsync(id);
            if (aisle == null)
            {
                return NotFound();
            }
            return View(aisle);
        }

        public async Task<IActionResult> Create()
        {
            await PopulateZonesDropdown();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Aisle aisle)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _aisleService.CreateAisleAsync(aisle);
                    TempData["Success"] = "Aisle created successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (ArgumentException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            await PopulateZonesDropdown();
            return View(aisle);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var aisle = await _aisleService.GetAisleByIdAsync(id);
            if (aisle == null)
            {
                return NotFound();
            }
            await PopulateZonesDropdown();
            return View(aisle);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Aisle aisle)
        {
            if (id != aisle.Id)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _aisleService.UpdateAisleAsync(aisle);
                    TempData["Success"] = "Aisle updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            await PopulateZonesDropdown();
            return View(aisle);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var aisle = await _aisleService.GetAisleByIdAsync(id);
            if (aisle == null)
            {
                return NotFound();
            }
            return View(aisle);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _aisleService.DeleteAisleAsync(id);
                TempData["Success"] = "Aisle deleted successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        private async Task PopulateZonesDropdown()
        {
            var zones = await _zoneService.GetAllZonesAsync();
            ViewBag.Zones = new SelectList(zones, "Id", "Name");
        }
    }
}

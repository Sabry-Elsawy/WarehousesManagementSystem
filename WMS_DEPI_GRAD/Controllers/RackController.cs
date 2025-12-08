using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WMS.BLL.Interfaces;
using WMS.DAL;

namespace WMS_DEPI_GRAD.Controllers
{
    [Authorize]
    public class RackController : Controller
    {
        private readonly IRackService _rackService;
        private readonly IAisleService _aisleService;

        public RackController(IRackService rackService, IAisleService aisleService)
        {
            _rackService = rackService;
            _aisleService = aisleService;
        }

        public async Task<IActionResult> Index()
        {
            var racks = await _rackService.GetAllRacksAsync();
            return View(racks);
        }

        public async Task<IActionResult> Details(int id)
        {
            var rack = await _rackService.GetRackByIdAsync(id);
            if (rack == null)
            {
                return NotFound();
            }
            return View(rack);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            await PopulateAislesDropdown();
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(Rack rack)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _rackService.CreateRackAsync(rack);
                    TempData["Success"] = "Rack created successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (ArgumentException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            await PopulateAislesDropdown();
            return View(rack);
        }

        [Authorize(Roles = "Admin,WarehouseManager")]
        public async Task<IActionResult> Edit(int id)
        {
            var rack = await _rackService.GetRackByIdAsync(id);
            if (rack == null)
            {
                return NotFound();
            }
            await PopulateAislesDropdown();
            return View(rack);
        }

        [Authorize(Roles = "Admin,WarehouseManager")]
        [HttpPost]
        public async Task<IActionResult> Edit(int id, Rack rack)
        {
            if (id != rack.Id)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _rackService.UpdateRackAsync(rack);
                    TempData["Success"] = "Rack updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            await PopulateAislesDropdown();
            return View(rack);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var rack = await _rackService.GetRackByIdAsync(id);
            if (rack == null)
            {
                return NotFound();
            }
            return View(rack);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _rackService.DeleteRackAsync(id);
                TempData["Success"] = "Rack deleted successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        private async Task PopulateAislesDropdown()
        {
            var aisles = await _aisleService.GetAllAislesAsync();
            ViewBag.Aisles = new SelectList(aisles, "Id", "Name");
        }
    }
}

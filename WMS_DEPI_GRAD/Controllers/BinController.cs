 
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Threading.Tasks;
using WMS.BLL.Interfaces;
using WMS.DAL;
using WMS.DAL.Entities;

namespace WMS_DEPI_GRAD.Controllers
{
    public class BinController : Controller
    {
        private readonly IBinService _binService;
        private readonly IRackService _rackService;
        private readonly IBinTypeService _binTypeService;

        public BinController(IBinService binService, IRackService rackService, IBinTypeService binTypeService)
        {
            _binService = binService;
            _rackService = rackService;
            _binTypeService = binTypeService;
        }

        public async Task<IActionResult> Index()
        {
            var bins = await _binService.GetAllBinsAsync();
            return View(bins);
        }

        public async Task<IActionResult> Create()
        {
            await PopulateDropdowns();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Bin bin)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _binService.CreateBinAsync(bin);
                    TempData["Success"] = "Bin created successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (ArgumentException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            await PopulateDropdowns();
            return View(bin);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var bin = await _binService.GetBinByIdAsync(id);
            if (bin == null) return NotFound();

            await PopulateDropdowns();
            return View(bin);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Bin bin)
        {
            if (id != bin.Id) return BadRequest();

            if (ModelState.IsValid)
            {
                try
                {
                    await _binService.UpdateBinAsync(bin);
                    TempData["Success"] = "Bin updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (ArgumentException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            await PopulateDropdowns();
            return View(bin);
        }

        public async Task<IActionResult> Details(int id)
        {
            var bin = await _binService.GetBinByIdAsync(id);
            if (bin == null) return NotFound();
            return View(bin);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var bin = await _binService.GetBinByIdAsync(id);
            if (bin == null) return NotFound();
            return View(bin);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _binService.DeleteBinAsync(id);
                TempData["Success"] = "Bin deleted successfully!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error deleting Bin: " + ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateDropdowns()
        {
            var racks = await _rackService.GetAllRacksAsync() ?? new List<Rack>();
            var binTypes = await _binTypeService.GetAllBinTypesAsync() ?? new List<BinType>();

            ViewBag.RackId = new SelectList(racks, "Id", "Code");
            ViewBag.BinTypeId = new SelectList(binTypes, "Id", "Name");
        }

    }
}
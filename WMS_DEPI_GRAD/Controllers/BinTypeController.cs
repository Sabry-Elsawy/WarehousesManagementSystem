using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using WMS.BLL.Interfaces;
using WMS.DAL.Entities;

namespace WMS_DEPI_GRAD.Controllers
{
    public class BinTypeController : Controller
    {
        private readonly IBinTypeService _binTypeService;

        public BinTypeController(IBinTypeService binTypeService)
        {
            _binTypeService = binTypeService;
        }

        public async Task<IActionResult> Index()
        {
            var binTypes = await _binTypeService.GetAllBinTypesAsync();
            return View(binTypes);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BinType binType)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _binTypeService.CreateBinTypeAsync(binType);
                    TempData["Success"] = "Bin Type created successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (ArgumentException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            return View(binType);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var binType = await _binTypeService.GetBinTypeByIdAsync(id);
            if (binType == null) return NotFound();
            return View(binType);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, BinType binType)
        {
            if (id != binType.Id) return BadRequest();

            if (ModelState.IsValid)
            {
                try
                {
                    await _binTypeService.UpdateBinTypeAsync(binType);
                    TempData["Success"] = "Bin Type updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (ArgumentException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            return View(binType);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var binType = await _binTypeService.GetBinTypeByIdAsync(id);
            if (binType == null) return NotFound();
            return View(binType);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _binTypeService.DeleteBinTypeAsync(id);
                TempData["Success"] = "Bin Type deleted successfully!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error deleting Bin Type: " + ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
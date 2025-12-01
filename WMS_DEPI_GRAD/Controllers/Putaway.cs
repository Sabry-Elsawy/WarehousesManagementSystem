using Microsoft.AspNetCore.Mvc;
using WMS.BLL.Interfaces;
using WMS.DAL.Entities;
using WMS_DEPI_GRAD.ViewModels.PutawayCircle;

namespace WMS_DEPI_GRAD.Controllers
{
    public class PutawayController : Controller
    {
        private readonly IPutawayService _putawayService;
        private readonly IBinService _binRepo;

        public PutawayController(IPutawayService putawayService, IBinService binRepo)
        {
            _putawayService = putawayService;
            _binRepo = binRepo;
        }

        public async Task<IActionResult> Index()
        {
            var putaways = await _putawayService.GetAllAsync();

            var vm = putaways.Select(p => new PutawayListVM
            {
                PutawayId = p.Id,
                SKU = p.ReceiptItem?.SKU ?? "",
                ProductName = p.ReceiptItem?.Product?.Name ?? "",
                Qty = p.Qty,
                Status = p.Status.ToString(),
                CreatedOn = p.CreatedOn
            }).ToList();

            

            return View(vm);
        }

        public async Task<IActionResult> Assign(int id)
        {
            var putaway = await _putawayService.GetByIdAsync(id);

            if (putaway == null)
                return NotFound();

            var bins = await _putawayService.GetAvailableBinsAsync(id);

            var vm = new PutawayAssignVM
            {
                PutawayId = id,
                SKU = putaway.ReceiptItem?.SKU ?? "",
                ProductName = putaway.ReceiptItem?.Product?.Name ?? "",
                Qty = putaway.Qty,
                Bins = bins.Select(b => new BinVM
                {
                    BinId = b.BinId,
                    Code = b.Code,
                    Capacity = b.Capacity,
                    Used = b.Used 
                }).ToList()
            };

            return View(vm);
        }

        [HttpPost]
public async Task<IActionResult> AssignPost(int putawayId, int binId, int qty)
{
    await _putawayService.AssignAsync(putawayId, binId, qty);
    return RedirectToAction("Confirm", new { id = putawayId, binId = binId });
}

        public async Task<IActionResult> Confirm(int id, int binId)
        {
            var putaway = await _putawayService.GetByIdAsync(id);
            
            var bin = await _binRepo.GetBinByIdAsync(binId);

            var vm = new PutawayConfirmVM
            {
                PutawayId = id,
                SKU = putaway.ReceiptItem.SKU,
                ProductName = putaway.ReceiptItem.Product.Name,
                Qty = putaway.Qty,
                BinCode = bin.Code
            };

            return View(vm);
        }


        [HttpPost]
        public async Task<IActionResult> ConfirmPost(int id)
        {
            await _putawayService.CompleteAsync(id);
            return RedirectToAction("Index");
        }
    }
}

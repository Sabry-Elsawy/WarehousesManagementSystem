using Microsoft.AspNetCore.Mvc;
using WMS.BLL.DTOs;
using WMS.BLL.Interfaces;

namespace WMS_DEPI_GRAD.Controllers;

public class CycleCountController : Controller
{
    private readonly ICycleCountService _cycleCountService;
    private readonly IProductService _productService; // Assuming these exist
    private readonly IBinService _binService;
    private readonly IZoneService _zoneService;

    public CycleCountController(
        ICycleCountService cycleCountService,
        IProductService productService,
        IBinService binService,
        IZoneService zoneService)
    {
        _cycleCountService = cycleCountService;
        _productService = productService;
        _binService = binService;
        _zoneService = zoneService;
    }

    public async Task<IActionResult> Index()
    {
        var sessions = await _cycleCountService.GetAllSessionsAsync();
        return View(sessions);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        // Populate dropdowns if needed
        ViewBag.Zones = await _zoneService.GetAllZonesAsync();
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateCycleCountDto model)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            var createdBy = User.Identity?.Name ?? "System";
            var id = await _cycleCountService.CreateSessionAsync(model, createdBy);
            return RedirectToAction("Execute", new { id });
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View(model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Execute(int id)
    {
        var session = await _cycleCountService.GetSessionAsync(id);
        if (session == null) return NotFound();

        var items = await _cycleCountService.GetSessionItemsAsync(id);
        ViewBag.Session = session;
        return View(items);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateCount([FromBody] UpdateCountDto model)
    {
        try
        {
            var success = await _cycleCountService.UpdateCountAsync(model);
            if (success) return Ok();
            return BadRequest("Failed to update count");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost]
    public async Task<IActionResult> Finalize(int id)
    {
        try
        {
            var performedBy = User.Identity?.Name ?? "System";
            var success = await _cycleCountService.FinalizeSessionAsync(id, performedBy);
            if (success)
            {
                TempData["Success"] = "Cycle Count finalized successfully!";
                return RedirectToAction("Index");
            }
            return BadRequest("Failed to finalize session");
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
            return RedirectToAction("Execute", new { id });
        }
    }

    public async Task<IActionResult> Details(int id)
    {
        var session = await _cycleCountService.GetSessionAsync(id);
        if (session == null) return NotFound();

        var items = await _cycleCountService.GetSessionItemsAsync(id);
        ViewBag.Session = session;
        return View(items);
    }
}

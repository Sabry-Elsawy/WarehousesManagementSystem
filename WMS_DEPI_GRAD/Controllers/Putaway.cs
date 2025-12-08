using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WMS.BLL.DTOs;
using WMS.BLL.Interfaces;
using WMS.DAL;
using WMS.DAL.Contract;
using WMS.DAL.UnitOfWork;
using WMS_DEPI_GRAD.Services;
using WMS_DEPI_GRAD.ViewModels.PutawayCircle;

namespace WMS_DEPI_GRAD.Controllers
{
    [Authorize(Roles = "Admin,InboundSpecialist")]
    public class PutawayController : Controller
    {
        private readonly IPutawayService _putawayService;
        private readonly IReceiptService _receiptService;
        private readonly ILoggedInUserService _userService;
        private readonly ILogger<PutawayController> _logger;

        public PutawayController(
            IPutawayService putawayService,
            IReceiptService receiptService,
            ILoggedInUserService userService,
            ILogger<PutawayController> logger)
        {
            _putawayService = putawayService;
            _receiptService = receiptService;
            _userService = userService;
            _logger = logger;
        }

        #region Index - List Putaways

        public async Task<IActionResult> Index(int page = 1, int pageSize = 10, string? search = null, string? status = null)
        {
            try
            {
                var (items, totalCount) = await _putawayService.GetPagedPutawaysAsync(page, pageSize, search, status);

                var viewModel = new PutawayIndexViewModel
                {
                    Items = items.Select(p => new PutawayListVM
                    {
                        PutawayId = p.Id,
                        SKU = p.SKU,
                        ProductName = p.ProductName,
                        Qty = p.Qty,
                        Status = p.Status.ToString(),
                        CreatedOn = p.CreatedOn
                    }).ToList(),
                    CurrentPage = page,
                    PageSize = pageSize,
                    TotalCount = totalCount,
                    TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                    SearchTerm = search,
                    StatusFilter = status
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading putaway list");
                TempData["Error"] = "Error loading putaways: " + ex.Message;
                return View(new PutawayIndexViewModel());
            }
        }

        #endregion

        #region Details

        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var putawayDto = await _putawayService.GetPutawayDetailsAsync(id);

                if (putawayDto == null)
                    return NotFound();

                var viewModel = new PutawayDetailsViewModel
                {
                    PutawayId = putawayDto.Id,
                    Qty = putawayDto.Qty,
                    Status = putawayDto.Status,
                    CreatedOn = putawayDto.CreatedOn,
                    AssignedOn = putawayDto.AssignedOn,
                    CompletedOn = putawayDto.CompletedOn,
                    ClosedOn = putawayDto.ClosedOn,
                    CreatedBy = putawayDto.CreatedBy,
                    PerformedBy = putawayDto.PerformedBy,
                    ClosedBy = putawayDto.ClosedBy,
                    SKU = putawayDto.SKU,
                    ProductName = putawayDto.ProductName,
                    BatchNumber = putawayDto.BatchNumber,
                    ExpiryDate = putawayDto.ExpiryDate,
                    AssignedBins = putawayDto.AssignedBins.Select(ab => new PutawayBinDetailVM
                    {
                        BinId = ab.BinId,
                        BinCode = ab.BinCode,
                        AssignedQty = ab.Qty,
                        AvailableCapacity = ab.AvailableCapacity,
                        Zone = ab.Zone
                    }).ToList()
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading putaway details {PutawayId}", id);
                TempData["Error"] = "Error loading putaway details: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        #endregion

        #region Create

        [Authorize(Roles = "Admin,InboundSpecialist")]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            try
            {
                // Get only OPEN receipts with available quantity
                var receipts = await _receiptService.GetAllAsync();
                var openReceipts = receipts.Where(r => r.Status == ReceiptStatus.Open).ToList();
                var availableItems = new List<ReceiptItemOption>();

                foreach (var receipt in openReceipts)
                {
                    var receiptDetails = await _receiptService.GetByIdAsync(receipt.Id);
                    if (receiptDetails?.ReceiptItems != null)
                    {
                        foreach (var item in receiptDetails.ReceiptItems)
                        {
                            // Calculate available qty (simplified - in real scenario, check putaway assignments)
                            int availableQty = item.QtyReceived;
                            if (availableQty > 0)
                            {
                                availableItems.Add(new ReceiptItemOption
                                {
                                    ReceiptItemId = item.Id,
                                    SKU = item.SKU ?? "",
                                    ProductName = item.Product?.Name ?? "",
                                    AvailableQty = availableQty
                                });
                            }
                        }
                    }
                }

                var viewModel = new CreatePutawayViewModel
                {
                    AvailableReceiptItems = availableItems
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading create putaway form");
                TempData["Error"] = "Error loading form: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        [Authorize(Roles = "Admin,InboundSpecialist")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreatePutawayViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    TempData["Error"] = "Invalid data provided";
                    return RedirectToAction(nameof(Create));
                }

                var userName = _userService.GetUserId();
                var putaway = await _putawayService.CreatePutawayForReceiptItemAsync(
                    model.SelectedReceiptItemId,
                    model.Qty,
                    userName);

                TempData["Success"] = $"Putaway task created successfully (ID: {putaway.Id})";
                return RedirectToAction(nameof(AutoAssign), new { id = putaway.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating putaway");
                TempData["Error"] = "Error creating putaway: " + ex.Message;
                return RedirectToAction(nameof(Create));
            }
        }

        #endregion

        #region Auto-Assign

        [Authorize(Roles = "Admin,InboundSpecialist")]
        [HttpGet]
        public async Task<IActionResult> AutoAssign(int id)
        {
            try
            {
                var result = await _putawayService.AutoAssignBinsAsync(id);

                var viewModel = new AutoAssignViewModel
                {
                    PutawayId = result.PutawayId,
                    TotalQty = result.TotalQty,
                    AssignedQty = result.AssignedQty,
                    FullyAssigned = result.FullyAssigned,
                    SuggestedBins = result.SuggestedBins,
                   WarningMessage = result.WarningMessage
                };

                // Get putaway details for SKU and product name
                var putaway = await _putawayService.GetPutawayDetailsAsync(id);
                if (putaway != null)
                {
                    viewModel.SKU = putaway.SKU;
                    viewModel.ProductName = putaway.ProductName;
                }

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error auto-assigning bins for putaway {PutawayId}", id);
                TempData["Error"] = "Error auto-assigning bins: " + ex.Message;
                return RedirectToAction(nameof(Details), new { id });
            }
        }

        [Authorize(Roles = "Admin,InboundSpecialist")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AcceptAutoAssign(int id, List<PutawayBinDto> suggestedBins)
        {
            try
            {
                var userName = _userService.GetUserId();
                await _putawayService.AssignBinsManualAsync(id, suggestedBins, userName);

                TempData["Success"] = "Bins assigned successfully!";
                return RedirectToAction(nameof(Details), new { id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error accepting auto-assign for putaway {PutawayId}", id);
                TempData["Error"] = "Error accepting assignments: " + ex.Message;
                return RedirectToAction(nameof(AutoAssign), new { id });
            }
        }

        #endregion

        #region Manual Assignment

        [Authorize(Roles = "Admin,InboundSpecialist")]
        [HttpGet]
        public async Task<IActionResult> AssignManual(int id)
        {
            try
            {
                var putaway = await _putawayService.GetPutawayDetailsAsync(id);
                if (putaway == null)
                    return NotFound();

                // Get available bins
                var availableBins = await _putawayService.GetAvailableBinsAsync(id);

                var viewModel = new AssignManualViewModel
                {
                    PutawayId = putaway.Id,
                    SKU = putaway.SKU,
                    ProductName = putaway.ProductName,
                    TotalQty = putaway.Qty,
                    AvailableBins = availableBins.Select(b => new BinCapacityDto
                    {
                        BinId = b.BinId,
                        Code = b.Code,
                        TotalCapacity = b.Capacity,
                        UsedCapacity = b.Used
                    }).ToList(),
                    CurrentAssignments = putaway.AssignedBins
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading manual assignment for putaway {PutawayId}", id);
                TempData["Error"] = "Error loading assignment form: " + ex.Message;
                return RedirectToAction(nameof(Details), new { id });
            }
        }

        [Authorize(Roles = "Admin,InboundSpecialist")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignManual(int id, List<PutawayBinDto> assignments)
        {
            try
            {
                var userName = _userService.GetUserId();
                await _putawayService.AssignBinsManualAsync(id, assignments, userName);

                TempData["Success"] = "Bins assigned manually!";
                return RedirectToAction(nameof(Details), new { id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving manual assignments for putaway {PutawayId}", id);
                TempData["Error"] = "Error saving assignments: " + ex.Message;
                return RedirectToAction(nameof(AssignManual), new { id });
            }
        }

        #endregion

        #region Execute

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Execute(int id)
        {
            try
            {
                var putaway = await _putawayService.GetPutawayDetailsAsync(id);
                if (putaway == null)
                    return NotFound();

                var viewModel = new ExecutePutawayViewModel
                {
                    PutawayId = putaway.Id,
                    SKU = putaway.SKU,
                    ProductName = putaway.ProductName,
                    TotalQty = putaway.Qty,
                    Assignments = putaway.AssignedBins
                };

                // Calculate before/after inventory (simplified)
                viewModel.AfterInventory = putaway.AssignedBins.Select(ab => new InventorySummaryDto
                {
                    BinId = ab.BinId,
                    BinCode = ab.BinCode,
                    CurrentQty = 0, // Would need to query actual inventory
                    ProjectedQty = ab.Qty
                }).ToList();

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading execute confirmation for putaway {PutawayId}", id);
                TempData["Error"] = "Error loading execution form: " + ex.Message;
                return RedirectToAction(nameof(Details), new { id });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExecuteConfirm(int id)
        {
            try
            {
                var userName = _userService.GetUserId();
                await _putawayService.ExecutePutawayAsync(id, userName);

                TempData["Success"] = "Putaway executed successfully! Inventory updated.";
                return RedirectToAction(nameof(Details), new { id });
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("modified by another user"))
            {
                TempData["Error"] = "Concurrency error: " + ex.Message;
                return RedirectToAction(nameof(Details), new { id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing putaway {PutawayId}", id);
                TempData["Error"] = "Error executing putaway: " + ex.Message;
                return RedirectToAction(nameof(Execute), new { id });
            }
        }

        #endregion

        #region Close

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Close(int id)
        {
            try
            {
                var userName = _userService.GetUserId();
                await _putawayService.ClosePutawayAsync(id, userName);

                TempData["Success"] = "Putaway closed successfully!";
                return RedirectToAction(nameof(Details), new { id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error closing putaway {PutawayId}", id);
                TempData["Error"] = "Error closing putaway: " + ex.Message;
                return RedirectToAction(nameof(Details), new { id });
            }
        }

        #endregion

        #region Legacy Actions (For Backwards Compatibility)

        [Authorize(Roles = "Admin,InboundSpecialist")]
        [HttpGet]
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

        [Authorize(Roles = "Admin,InboundSpecialist")]
        [HttpPost]
        public async Task<IActionResult> AssignPost(int putawayId, int binId, int qty)
        {
            await _putawayService.AssignAsync(putawayId, binId, qty);
            return RedirectToAction("Confirm", new { id = putawayId, binId = binId });
        }

        [HttpGet]
        public async Task<IActionResult> Confirm(int id, int binId)
        {
            var putaway = await _putawayService.GetByIdAsync(id);

            if (putaway?.ReceiptItem?.Product == null)
                return NotFound();

            var binRepo = _unitOfWork.GetRepository<WMS.DAL.Entities.Bin, int>();
            var bin = await binRepo.GetByIdAsync(binId);

            if (bin == null)
                return NotFound();

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

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> ConfirmPost(int id)
        {
            await _putawayService.CompleteAsync(id);
            return RedirectToAction("Index");
        }

        #endregion

        private IUnitOfWork _unitOfWork => HttpContext.RequestServices.GetRequiredService<WMS.DAL.UnitOfWork.IUnitOfWork>();
    }
}

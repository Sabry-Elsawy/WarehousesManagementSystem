using Microsoft.AspNetCore.Mvc;
using WMS.BLL.Interfaces;
using WMS.DAL;
using WMS_DEPI_GRAD.ViewModels;

namespace WMS_DEPI_GRAD.Controllers;

public class SupplierController : Controller
{
    private readonly ISupplierService _supplierService;

    public SupplierController(ISupplierService supplierService)
    {
        _supplierService = supplierService;
    }

    // GET: Supplier
    public async Task<IActionResult> Index(int page = 1, string? search = null, bool? active = null, string sortBy = "Name", string sortOrder = "asc")
    {
        const int pageSize = 10;
        var (suppliers, totalCount) = await _supplierService.GetPagedListAsync(page, pageSize, search, active, sortBy, sortOrder);

        var viewModel = new SupplierListViewModel
        {
            Suppliers = suppliers.Select(s => new SupplierViewModel
            {
                Id = s.Id,
                Name = s.Name,
                Address = s.Address,
                ContactEmail = s.ContactEmail,
                ContactPhone = s.ContactPhone,
                IsActive = s.IsActive,
                CreatedOn = s.CreatedOn,
                CreatedBy = s.CreatedBy
            }).ToList(),
            CurrentPage = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            SearchTerm = search,
            FilterActive = active,
            SortBy = sortBy,
            SortOrder = sortOrder
        };

        return View(viewModel);
    }

    // GET: Supplier/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var supplier = await _supplierService.GetByIdAsync(id);
        if (supplier == null)
            return NotFound();

        var viewModel = new SupplierViewModel
        {
            Id = supplier.Id,
            Name = supplier.Name,
            Address = supplier.Address,
            ContactEmail = supplier.ContactEmail,
            ContactPhone = supplier.ContactPhone,
            IsActive = supplier.IsActive,
            CreatedOn = supplier.CreatedOn,
            CreatedBy = supplier.CreatedBy
        };

        return View(viewModel);
    }

    // GET: Supplier/Create
    public IActionResult Create()
    {
        return View(new CreateSupplierViewModel());
    }

    // POST: Supplier/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateSupplierViewModel viewModel)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var supplier = new Vendor
                {
                    Name = viewModel.Name,
                    Address = viewModel.Address,
                    ContactEmail = viewModel.ContactEmail,
                    ContactPhone = viewModel.ContactPhone,
                    IsActive = viewModel.IsActive
                };

                await _supplierService.CreateAsync(supplier);
                TempData["Success"] = "Supplier created successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
        }

        return View(viewModel);
    }

    // GET: Supplier/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var supplier = await _supplierService.GetByIdAsync(id);
        if (supplier == null)
            return NotFound();

        var viewModel = new SupplierViewModel
        {
            Id = supplier.Id,
            Name = supplier.Name,
            Address = supplier.Address,
            ContactEmail = supplier.ContactEmail,
            ContactPhone = supplier.ContactPhone,
            IsActive = supplier.IsActive
        };

        return View(viewModel);
    }

    // POST: Supplier/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, SupplierViewModel viewModel)
    {
        if (id != viewModel.Id)
            return BadRequest();

        if (ModelState.IsValid)
        {
            try
            {
                var supplier = await _supplierService.GetByIdAsync(id);
                if (supplier == null)
                    return NotFound();

                supplier.Name = viewModel.Name;
                supplier.Address = viewModel.Address;
                supplier.ContactEmail = viewModel.ContactEmail;
                supplier.ContactPhone = viewModel.ContactPhone;
                supplier.IsActive = viewModel.IsActive;

                await _supplierService.UpdateAsync(supplier);
                TempData["Success"] = "Supplier updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
        }

        return View(viewModel);
    }

    // GET: Supplier/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
        var supplier = await _supplierService.GetByIdAsync(id);
        if (supplier == null)
            return NotFound();

        var viewModel = new SupplierViewModel
        {
            Id = supplier.Id,
            Name = supplier.Name,
            Address = supplier.Address,
            ContactEmail = supplier.ContactEmail,
            ContactPhone = supplier.ContactPhone,
            IsActive = supplier.IsActive
        };

        return View(viewModel);
    }

    // POST: Supplier/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            await _supplierService.ToggleActiveAsync(id);
            TempData["Success"] = "Supplier status changed successfully!";
        }
        catch (ArgumentException ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }
}

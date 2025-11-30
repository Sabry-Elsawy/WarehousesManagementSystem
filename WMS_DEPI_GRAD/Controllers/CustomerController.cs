using Microsoft.AspNetCore.Mvc;
using WMS.BLL.Interfaces;
using WMS.DAL;
using WMS_DEPI_GRAD.ViewModels;

namespace WMS_DEPI_GRAD.Controllers;

public class CustomerController : Controller
{
    private readonly ICustomerService _customerService;

    public CustomerController(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    // GET: Customer
    public async Task<IActionResult> Index(int page = 1, string? search = null, bool? active = null, string sortBy = "Name", string sortOrder = "asc")
    {
        const int pageSize = 10;
        var (customers, totalCount) = await _customerService.GetPagedListAsync(page, pageSize, search, active, sortBy, sortOrder);

        var viewModel = new CustomerListViewModel
        {
            Customers = customers.Select(c => new CustomerViewModel
            {
                Id = c.Id,
                Name = c.Name,
                Address = c.Address,
                Phone = c.Phone,
                Email = c.Email,
                IsActive = c.IsActive,
                CreatedOn = c.CreatedOn,
                CreatedBy = c.CreatedBy
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

    // GET: Customer/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var customer = await _customerService.GetByIdAsync(id);
        if (customer == null)
            return NotFound();

        var viewModel = new CustomerViewModel
        {
            Id = customer.Id,
            Name = customer.Name,
            Address = customer.Address,
            Phone = customer.Phone,
            Email = customer.Email,
            IsActive = customer.IsActive,
            CreatedOn = customer.CreatedOn,
            CreatedBy = customer.CreatedBy
        };

        return View(viewModel);
    }

    // GET: Customer/Create
    public IActionResult Create()
    {
        return View(new CreateCustomerViewModel());
    }

    // POST: Customer/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateCustomerViewModel viewModel)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var customer = new Customer
                {
                    Name = viewModel.Name,
                    Address = viewModel.Address,
                    Phone = viewModel.Phone,
                    Email = viewModel.Email,
                    IsActive = viewModel.IsActive
                };

                await _customerService.CreateAsync(customer);
                TempData["Success"] = "Customer created successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
        }

        return View(viewModel);
    }

    // GET: Customer/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var customer = await _customerService.GetByIdAsync(id);
        if (customer == null)
            return NotFound();

        var viewModel = new CustomerViewModel
        {
            Id = customer.Id,
            Name = customer.Name,
            Address = customer.Address,
            Phone = customer.Phone,
            Email = customer.Email,
            IsActive = customer.IsActive
        };

        return View(viewModel);
    }

    // POST: Customer/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, CustomerViewModel viewModel)
    {
        if (id != viewModel.Id)
            return BadRequest();

        if (ModelState.IsValid)
        {
            try
            {
                var customer = await _customerService.GetByIdAsync(id);
                if (customer == null)
                    return NotFound();

                customer.Name = viewModel.Name;
                customer.Address = viewModel.Address;
                customer.Phone = viewModel.Phone;
                customer.Email = viewModel.Email;
                customer.IsActive = viewModel.IsActive;

                await _customerService.UpdateAsync(customer);
                TempData["Success"] = "Customer updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
        }

        return View(viewModel);
    }

    // GET: Customer/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
        var customer = await _customerService.GetByIdAsync(id);
        if (customer == null)
            return NotFound();

        var viewModel = new CustomerViewModel
        {
            Id = customer.Id,
            Name = customer.Name,
            Address = customer.Address,
            Phone = customer.Phone,
            Email = customer.Email,
            IsActive = customer.IsActive
        };

        return View(viewModel);
    }

    // POST: Customer/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            await _customerService.ToggleActiveAsync(id);
            TempData["Success"] = "Customer status changed successfully!";
        }
        catch (ArgumentException ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }
}

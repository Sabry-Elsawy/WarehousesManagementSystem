using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WMS.BLL.Interfaces;
using WMS.DAL;
using WMS_DEPI_GRAD.ViewModels;

namespace WMS_DEPI_GRAD.Controllers;

[Authorize(Roles = "Admin")]
public class ProductController : Controller
{
    private readonly IProductService _productService;

    public ProductController(IProductService productService)
    {
        _productService = productService;
    }

    // GET: Product
    public async Task<IActionResult> Index(int page = 1, string? search = null, bool? active = null, string sortBy = "Name", string sortOrder = "asc")
    {
        const int pageSize = 10;
        var (products, totalCount) = await _productService.GetPagedListAsync(page, pageSize, search, active, sortBy, sortOrder);

        var viewModel = new ProductListViewModel
        {
            Products = products.Select(p => new ProductViewModel
            {
                Id = p.Id,
                Code = p.Code,
                Name = p.Name,
                Description = p.Description,
                UnitOfMeasure = p.UnitOfMeasure,
                Weight = p.Weight,
                Volume = p.Volume,
                Barcode = p.Barcode,
                IsActive = p.IsActive,
                CreatedOn = p.CreatedOn,
                CreatedBy = p.CreatedBy
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

    // GET: Product/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var product = await _productService.GetByIdAsync(id);
        if (product == null)
            return NotFound();

        var viewModel = new ProductViewModel
        {
            Id = product.Id,
            Code = product.Code,
            Name = product.Name,
            Description = product.Description,
            UnitOfMeasure = product.UnitOfMeasure,
            Weight = product.Weight,
            Volume = product.Volume,
            Barcode = product.Barcode,
            IsActive = product.IsActive,
            CreatedOn = product.CreatedOn,
            CreatedBy = product.CreatedBy
        };

        return View(viewModel);
    }

    // GET: Product/Create
    public IActionResult Create()
    {
        return View(new CreateProductViewModel());
    }

    // POST: Product/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateProductViewModel viewModel)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var product = new Product
                {
                    Code = viewModel.Code,
                    Name = viewModel.Name,
                    Description = viewModel.Description,
                    UnitOfMeasure = viewModel.UnitOfMeasure,
                    Weight = viewModel.Weight,
                    Volume = viewModel.Volume,
                    Barcode = viewModel.Barcode,
                    IsActive = viewModel.IsActive
                };

                await _productService.CreateAsync(product);
                TempData["Success"] = "Product created successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
        }

        return View(viewModel);
    }

    // GET: Product/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var product = await _productService.GetByIdAsync(id);
        if (product == null)
            return NotFound();

        var viewModel = new ProductViewModel
        {
            Id = product.Id,
            Code = product.Code,
            Name = product.Name,
            Description = product.Description,
            UnitOfMeasure = product.UnitOfMeasure,
            Weight = product.Weight,
            Volume = product.Volume,
            Barcode = product.Barcode,
            IsActive = product.IsActive
        };

        return View(viewModel);
    }

    // POST: Product/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ProductViewModel viewModel)
    {
        if (id != viewModel.Id)
            return BadRequest();

        if (ModelState.IsValid)
        {
            try
            {
                var product = await _productService.GetByIdAsync(id);
                if (product == null)
                    return NotFound();

                product.Code = viewModel.Code;
                product.Name = viewModel.Name;
                product.Description = viewModel.Description;
                product.UnitOfMeasure = viewModel.UnitOfMeasure;
                product.Weight = viewModel.Weight;
                product.Volume = viewModel.Volume;
                product.Barcode = viewModel.Barcode;
                product.IsActive = viewModel.IsActive;

                await _productService.UpdateAsync(product);
                TempData["Success"] = "Product updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
        }

        return View(viewModel);
    }

    // GET: Product/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
        var product = await _productService.GetByIdAsync(id);
        if (product == null)
            return NotFound();

        var viewModel = new ProductViewModel
        {
            Id = product.Id,
            Code = product.Code,
            Name = product.Name,
            Description = product.Description,
            UnitOfMeasure = product.UnitOfMeasure,
            Weight = product.Weight,
            Volume = product.Volume,
            Barcode = product.Barcode,
            IsActive = product.IsActive
        };

        return View(viewModel);
    }

    // POST: Product/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            await _productService.ToggleActiveAsync(id);
            TempData["Success"] = "Product status changed successfully!";
        }
        catch (ArgumentException ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }
}

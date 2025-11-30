using WMS.BLL.Interfaces;
using WMS.DAL;
using WMS.DAL.UnitOfWork;

namespace WMS.BLL.Services;

public class ProductService : IProductService
{
    private readonly IUnitOfWork _unitOfWork;

    public ProductService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyList<Product>> GetAllAsync()
    {
        var repo = _unitOfWork.GetRepository<Product, int>();
        return await repo.GetAllAsync();
    }

    public async Task<(IReadOnlyList<Product> Items, int TotalCount)> GetPagedListAsync(
        int pageNumber,
        int pageSize,
        string? searchTerm = null,
        bool? filterActive = null,
        string sortBy = "Name",
        string sortOrder = "asc")
    {
        var repo = _unitOfWork.GetRepository<Product, int>();

        // Build filter expression
        System.Linq.Expressions.Expression<Func<Product, bool>>? filter = null;
        if (!string.IsNullOrWhiteSpace(searchTerm) || filterActive.HasValue)
        {
            filter = p =>
                (string.IsNullOrWhiteSpace(searchTerm) ||
                 p.Name.Contains(searchTerm) ||
                 p.Code.Contains(searchTerm)) &&
                (!filterActive.HasValue || p.IsActive == filterActive.Value);
        }

        // Build sort expression
        Func<IQueryable<Product>, IOrderedQueryable<Product>>? orderBy = sortBy.ToLower() switch
        {
            "code" => sortOrder == "desc"
                ? q => q.OrderByDescending(p => p.Code)
                : q => q.OrderBy(p => p.Code),
            "createdon" => sortOrder == "desc"
                ? q => q.OrderByDescending(p => p.CreatedOn)
                : q => q.OrderBy(p => p.CreatedOn),
            _ => sortOrder == "desc"
                ? q => q.OrderByDescending(p => p.Name)
                : q => q.OrderBy(p => p.Name)
        };

        return await repo.GetPagedListAsync(pageNumber, pageSize, filter, orderBy);
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        var repo = _unitOfWork.GetRepository<Product, int>();
        return await repo.GetByIdAsync(id);
    }

    public async Task CreateAsync(Product product)
    {
        // Validate unique code
        if (!await IsCodeUniqueAsync(product.Code))
            throw new ArgumentException($"Product with code '{product.Code}' already exists.");

        var repo = _unitOfWork.GetRepository<Product, int>();
        await repo.AddAsync(product);
        await _unitOfWork.CompleteAsync();
    }

    public async Task UpdateAsync(Product product)
    {
        // Validate unique code
        if (!await IsCodeUniqueAsync(product.Code, product.Id))
            throw new ArgumentException($"Product with code '{product.Code}' already exists.");

        var repo = _unitOfWork.GetRepository<Product, int>();
        repo.Update(product);
        await _unitOfWork.CompleteAsync();
    }

    public async Task ToggleActiveAsync(int id)
    {
        var repo = _unitOfWork.GetRepository<Product, int>();
        var product = await repo.GetByIdAsync(id);
        
        if (product == null)
            throw new ArgumentException($"Product with ID {id} not found.");

        product.IsActive = !product.IsActive;
        repo.Update(product);
        await _unitOfWork.CompleteAsync();
    }

    public async Task<bool> IsCodeUniqueAsync(string code, int? excludeId = null)
    {
        var repo = _unitOfWork.GetRepository<Product, int>();
        var (products, _) = await repo.GetPagedListAsync(
            1, 
            1, 
            p => p.Code == code && (!excludeId.HasValue || p.Id != excludeId.Value));
        
        return products.Count == 0;
    }
}

using WMS.DAL;

namespace WMS.BLL.Interfaces;

public interface IProductService
{
    Task<IReadOnlyList<Product>> GetAllAsync();
    Task<(IReadOnlyList<Product> Items, int TotalCount)> GetPagedListAsync(
        int pageNumber,
        int pageSize,
        string? searchTerm = null,
        bool? filterActive = null,
        string sortBy = "Name",
        string sortOrder = "asc");
    Task<Product?> GetByIdAsync(int id);
    Task CreateAsync(Product product);
    Task UpdateAsync(Product product);
    Task ToggleActiveAsync(int id);
    Task<bool> IsCodeUniqueAsync(string code, int? excludeId = null);
}

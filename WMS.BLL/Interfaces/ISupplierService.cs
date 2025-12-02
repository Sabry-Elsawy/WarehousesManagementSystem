using WMS.DAL;

namespace WMS.BLL.Interfaces;

public interface ISupplierService
{
    Task<IReadOnlyList<Vendor>> GetAllAsync();
    Task<(IReadOnlyList<Vendor> Items, int TotalCount)> GetPagedListAsync(
        int pageNumber,
        int pageSize,
        string? searchTerm = null,
        bool? filterActive = null,
        string sortBy = "Name",
        string sortOrder = "asc");
    Task<Vendor?> GetByIdAsync(int id);
    Task CreateAsync(Vendor supplier);
    Task UpdateAsync(Vendor supplier);
    Task ToggleActiveAsync(int id);
    Task<bool> IsNameUniqueAsync(string name, int? excludeId = null);
}

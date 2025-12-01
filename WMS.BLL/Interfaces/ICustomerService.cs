using WMS.DAL;

namespace WMS.BLL.Interfaces;

public interface ICustomerService
{
    Task<IReadOnlyList<Customer>> GetAllAsync();
    Task<(IReadOnlyList<Customer> Items, int TotalCount)> GetPagedListAsync(
        int pageNumber,
        int pageSize,
        string? searchTerm = null,
        bool? filterActive = null,
        string sortBy = "Name",
        string sortOrder = "asc");
    Task<Customer?> GetByIdAsync(int id);
    Task CreateAsync(Customer customer);
    Task UpdateAsync(Customer customer);
    Task ToggleActiveAsync(int id);
    Task<bool> IsEmailUniqueAsync(string email, int? excludeId = null);
}

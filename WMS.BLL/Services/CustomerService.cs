using WMS.BLL.Interfaces;
using WMS.DAL;
using WMS.DAL.UnitOfWork;

namespace WMS.BLL.Services;

public class CustomerService : ICustomerService
{
    private readonly IUnitOfWork _unitOfWork;

    public CustomerService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyList<Customer>> GetAllAsync()
    {
        var repo = _unitOfWork.GetRepository<Customer, int>();
        return await repo.GetAllAsync();
    }

    public async Task<(IReadOnlyList<Customer> Items, int TotalCount)> GetPagedListAsync(
        int pageNumber,
        int pageSize,
        string? searchTerm = null,
        bool? filterActive = null,
        string sortBy = "Name",
        string sortOrder = "asc")
    {
        var repo = _unitOfWork.GetRepository<Customer, int>();

        // Build filter expression
        System.Linq.Expressions.Expression<Func<Customer, bool>>? filter = null;
        if (!string.IsNullOrWhiteSpace(searchTerm) || filterActive.HasValue)
        {
            filter = c =>
                (string.IsNullOrWhiteSpace(searchTerm) ||
                 c.Name.Contains(searchTerm) ||
                 c.Email.Contains(searchTerm)) &&
                (!filterActive.HasValue || c.IsActive == filterActive.Value);
        }

        // Build sort expression
        Func<IQueryable<Customer>, IOrderedQueryable<Customer>>? orderBy = sortBy.ToLower() switch
        {
            "createdon" => sortOrder == "desc"
                ? q => q.OrderByDescending(c => c.CreatedOn)
                : q => q.OrderBy(c => c.CreatedOn),
            _ => sortOrder == "desc"
                ? q => q.OrderByDescending(c => c.Name)
                : q => q.OrderBy(c => c.Name)
        };

        return await repo.GetPagedListAsync(pageNumber, pageSize, filter, orderBy);
    }

    public async Task<Customer?> GetByIdAsync(int id)
    {
        var repo = _unitOfWork.GetRepository<Customer, int>();
        return await repo.GetByIdAsync(id);
    }

    public async Task CreateAsync(Customer customer)
    {
        // Validate unique email
        if (!await IsEmailUniqueAsync(customer.Email))
            throw new ArgumentException($"Customer with email '{customer.Email}' already exists.");

        var repo = _unitOfWork.GetRepository<Customer, int>();
        await repo.AddAsync(customer);
        await _unitOfWork.CompleteAsync();
    }

    public async Task UpdateAsync(Customer customer)
    {
        // Validate unique email
        if (!await IsEmailUniqueAsync(customer.Email, customer.Id))
            throw new ArgumentException($"Customer with email '{customer.Email}' already exists.");

        var repo = _unitOfWork.GetRepository<Customer, int>();
        repo.Update(customer);
        await _unitOfWork.CompleteAsync();
    }

    public async Task ToggleActiveAsync(int id)
    {
        var repo = _unitOfWork.GetRepository<Customer, int>();
        var customer = await repo.GetByIdAsync(id);
        
        if (customer == null)
            throw new ArgumentException($"Customer with ID {id} not found.");

        customer.IsActive = !customer.IsActive;
        repo.Update(customer);
        await _unitOfWork.CompleteAsync();
    }

    public async Task<bool> IsEmailUniqueAsync(string email, int? excludeId = null)
    {
        var repo = _unitOfWork.GetRepository<Customer, int>();
        var (customers, _) = await repo.GetPagedListAsync(
            1, 
            1, 
            c => c.Email == email && (!excludeId.HasValue || c.Id != excludeId.Value));
        
        return customers.Count == 0;
    }
}

using WMS.BLL.Interfaces;
using WMS.DAL;
using WMS.DAL.UnitOfWork;

namespace WMS.BLL.Services;

public class SupplierService : ISupplierService
{
    private readonly IUnitOfWork _unitOfWork;

    public SupplierService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyList<Vendor>> GetAllAsync()
    {
        var repo = _unitOfWork.GetRepository<Vendor, int>();
        return await repo.GetAllAsync();
    }

    public async Task<(IReadOnlyList<Vendor> Items, int TotalCount)> GetPagedListAsync(
        int pageNumber,
        int pageSize,
        string? searchTerm = null,
        bool? filterActive = null,
        string sortBy = "Name",
        string sortOrder = "asc")
    {
        var repo = _unitOfWork.GetRepository<Vendor, int>();

        // Build filter expression
        System.Linq.Expressions.Expression<Func<Vendor, bool>>? filter = null;
        if (!string.IsNullOrWhiteSpace(searchTerm) || filterActive.HasValue)
        {
            filter = v =>
                (string.IsNullOrWhiteSpace(searchTerm) ||
                 v.Name.Contains(searchTerm) ||
                 v.ContactEmail.Contains(searchTerm)) &&
                (!filterActive.HasValue || v.IsActive == filterActive.Value);
        }

        // Build sort expression
        Func<IQueryable<Vendor>, IOrderedQueryable<Vendor>>? orderBy = sortBy.ToLower() switch
        {
            "createdon" => sortOrder == "desc"
                ? q => q.OrderByDescending(v => v.CreatedOn)
                : q => q.OrderBy(v => v.CreatedOn),
            _ => sortOrder == "desc"
                ? q => q.OrderByDescending(v => v.Name)
                : q => q.OrderBy(v => v.Name)
        };

        return await repo.GetPagedListAsync(pageNumber, pageSize, filter, orderBy);
    }

    public async Task<Vendor?> GetByIdAsync(int id)
    {
        var repo = _unitOfWork.GetRepository<Vendor, int>();
        return await repo.GetByIdAsync(id);
    }

    public async Task CreateAsync(Vendor supplier)
    {
        // Validate unique name
        if (!await IsNameUniqueAsync(supplier.Name))
            throw new ArgumentException($"Supplier with name '{supplier.Name}' already exists.");

        var repo = _unitOfWork.GetRepository<Vendor, int>();
        await repo.AddAsync(supplier);
        await _unitOfWork.CompleteAsync();
    }

    public async Task UpdateAsync(Vendor supplier)
    {
        // Validate unique name
        if (!await IsNameUniqueAsync(supplier.Name, supplier.Id))
            throw new ArgumentException($"Supplier with name '{supplier.Name}' already exists.");

        var repo = _unitOfWork.GetRepository<Vendor, int>();
        repo.Update(supplier);
        await _unitOfWork.CompleteAsync();
    }

    public async Task ToggleActiveAsync(int id)
    {
        var repo = _unitOfWork.GetRepository<Vendor, int>();
        var supplier = await repo.GetByIdAsync(id);
        
        if (supplier == null)
            throw new ArgumentException($"Supplier with ID {id} not found.");

        supplier.IsActive = !supplier.IsActive;
        repo.Update(supplier);
        await _unitOfWork.CompleteAsync();
    }

    public async Task<bool> IsNameUniqueAsync(string name, int? excludeId = null)
    {
        var repo = _unitOfWork.GetRepository<Vendor, int>();
        var (suppliers, _) = await repo.GetPagedListAsync(
            1, 
            1, 
            v => v.Name == name && (!excludeId.HasValue || v.Id != excludeId.Value));
        
        return suppliers.Count == 0;
    }
}

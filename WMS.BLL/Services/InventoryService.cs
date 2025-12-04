using Microsoft.EntityFrameworkCore;
using WMS.BLL.DTOs;
using WMS.BLL.Interfaces;
using WMS.DAL;
using WMS.DAL.Entities;
using WMS.DAL.UnitOfWork;

namespace WMS.BLL.Services;
public class InventoryService:IInventoryService
{
    private readonly IUnitOfWork _unitOfWork;

    public InventoryService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<InventoryListDto>> GetAllAsync(string? search = null)
    {
        var repo = _unitOfWork.GetRepository<Inventory, int>();

        var data = await repo.GetAllWithIncludeAsync(
            withTracking: false,
            include: q => q
                .Include(i => i.Product)
                .Include(i => i.Bin)
        );

        
        var list = data.Select(i => new InventoryListDto
        {
            SKU = i.Product.Code,
            ProductName = i.Product.Name,
            Quantity = i.Quantity,
            Location = i.Bin.Code,
            Status = i.Status,
            LastUpdated = i.LastModifiedOn ?? i.CreatedOn
        });

        
        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim().ToLower();

            list = list.Where(x =>
                   x.SKU.ToLower().Contains(s)
                || x.ProductName.ToLower().Contains(s)
                || x.Location.ToLower().Contains(s)
                || x.Status.ToLower().Contains(s)
            );
        }

        return list.OrderByDescending(x => x.LastUpdated).ToList();
    }

    public async Task<bool> AddInventoryAsync(AddInventoryDto dto)
    {
        
        var productRepo = _unitOfWork.GetRepository<Product, int>();
        var product = (await productRepo.GetAllAsync())
            .FirstOrDefault(p => p.Name.ToLower() == dto.ProductName.ToLower());

        if (product == null)
            throw new Exception("Product not found. Create product first.");

        
        var binRepo = _unitOfWork.GetRepository<Bin, int>();
        var bin = (await binRepo.GetAllAsync())
            .FirstOrDefault(b => b.Code.ToLower() == dto.Location.ToLower());

        if (bin == null)
            throw new Exception("Bin not found.");

        
        var inventoryRepo = _unitOfWork.GetRepository<Inventory, int>();

        var inv = new Inventory
        {
            ProductId = product.Id,
            BinId = bin.Id,
            Quantity = dto.Quantity,
            Status = dto.Status,
            CreatedOn = DateTime.Now,
            BatchNumber = "",
            ExpiryDate = ""
        };

        await inventoryRepo.AddAsync(inv);
        await _unitOfWork.CompleteAsync();

        return true;
    }
    public async Task<bool> AdjustInventoryAsync(AdjustInventoryDto dto)
    {
        
        var productRepo = _unitOfWork.GetRepository<Product, int>();
        var product = (await productRepo.GetAllAsync())
            .FirstOrDefault(p => p.Code.ToLower() == dto.SKU.ToLower());

        if (product == null)
            throw new Exception("SKU not found.");

        
        var invRepo = _unitOfWork.GetRepository<Inventory, int>();
        var inventories = await invRepo.GetAllAsync();

        var inv = inventories.FirstOrDefault(i => i.ProductId == product.Id);

        if (inv == null)
            throw new Exception("No inventory found for this SKU.");

        
        int newQty = inv.Quantity + dto.Change;

        if (newQty < 0)
            throw new Exception("Quantity cannot be negative.");

        inv.Quantity = newQty;
        inv.LastModifiedOn = DateTime.Now;

        invRepo.Update(inv);
        await _unitOfWork.CompleteAsync();

        return true;
    }

}

using WMS.BLL.Interfaces;
using WMS.DAL;
using WMS.DAL.UnitOfWork;

namespace WMS.BLL.Services
{
    public class WarehouseService : IWarehouseService
    {
        private readonly IUnitOfWork _unitOfWork;

        public WarehouseService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Warehouse> CreateWarehouseAsync(Warehouse warehouse)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(warehouse.Name))
                throw new ArgumentException("Warehouse Name is required.");
            if (string.IsNullOrWhiteSpace(warehouse.Code))
                throw new ArgumentException("Warehouse Code is required.");
            if (warehouse.Capacity <= 0)
                throw new ArgumentException("Capacity must be greater than zero.");
            if (string.IsNullOrWhiteSpace(warehouse.City) || string.IsNullOrWhiteSpace(warehouse.Street))
                throw new ArgumentException("Location details (City, Street) are required.");

            var warehouseRepository = _unitOfWork.GetRepository<Warehouse, int>();
            
            // Add to Repository
            await warehouseRepository.AddAsync(warehouse);
            
            // Commit Transaction
            await _unitOfWork.CompleteAsync();

            return warehouse;
        }

        public async Task<Warehouse?> GetWarehouseByIdAsync(int id)
        {
            var warehouseRepository = _unitOfWork.GetRepository<Warehouse, int>();
            return await warehouseRepository.GetByIdAsync(id);
        }

        public async Task<IReadOnlyList<Warehouse>> GetAllWarehousesAsync()
        {
            var warehouseRepository = _unitOfWork.GetRepository<Warehouse, int>();
            return await warehouseRepository.GetAllAsync( false);
        }

        public async Task<Warehouse> UpdateWarehouseAsync(Warehouse warehouse)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(warehouse.Name))
                throw new ArgumentException("Warehouse Name is required.");
            if (warehouse.Capacity <= 0)
                throw new ArgumentException("Capacity must be greater than zero.");

            var warehouseRepository = _unitOfWork.GetRepository<Warehouse, int>();
            var existingWarehouse = await warehouseRepository.GetByIdAsync(warehouse.Id);
            
            if (existingWarehouse == null)
                throw new KeyNotFoundException($"Warehouse with ID {warehouse.Id} not found.");

            // Update fields
            existingWarehouse.Name = warehouse.Name;
            existingWarehouse.Code = warehouse.Code;
            existingWarehouse.Capacity = warehouse.Capacity;
            existingWarehouse.City = warehouse.City;
            existingWarehouse.Country = warehouse.Country;
            existingWarehouse.Street = warehouse.Street;
            
            // Update Repository
            warehouseRepository.Update(existingWarehouse);

            // Commit Transaction
            await _unitOfWork.CompleteAsync();

            return existingWarehouse;
        }

        public async Task DeleteWarehouseAsync(int id)
        {
            var warehouseRepository = _unitOfWork.GetRepository<Warehouse, int>();
            var warehouse = await warehouseRepository.GetByIdAsync(id);
            
            if (warehouse == null)
                throw new KeyNotFoundException($"Warehouse with ID {id} not found.");

            // Check Dependencies - using Zone repository
            var zoneRepository = _unitOfWork.GetRepository<Zone, int>();
            var zones = await zoneRepository.GetAllAsync( false);
            var hasZones = zones.Any(z => z.WarehouseId == id);
            
            if (hasZones)
                throw new InvalidOperationException("Cannot delete Warehouse because it has related Zones. Please delete the Zones first.");

            // Delete
            warehouseRepository.Delete(warehouse);

            // Commit Transaction
            await _unitOfWork.CompleteAsync();
        }
    }
}

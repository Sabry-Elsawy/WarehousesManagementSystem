using Microsoft.EntityFrameworkCore;
using WMS.BLL.Interfaces;
using WMS.DAL;
using WMS.DAL.UnitOfWork;

namespace WMS.BLL.Services
{
    public class ZoneService : IZoneService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ZoneService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Zone> CreateZoneAsync(Zone zone)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(zone.Name))
                throw new ArgumentException("Zone Name is required.");
            if (zone.WarehouseId <= 0)
                throw new ArgumentException("Valid Warehouse must be selected.");

            var zoneRepository = _unitOfWork.GetRepository<Zone, int>();
            
            await zoneRepository.AddAsync(zone);
            
            await _unitOfWork.CompleteAsync();

            return zone;
        }

        public async Task<Zone?> GetZoneByIdAsync(int id)
        {
            var zoneRepository = _unitOfWork.GetRepository<Zone, int>();
            return await zoneRepository.GetByIdAsync(id);
        }

        public async Task<IReadOnlyList<Zone>> GetAllZonesAsync()
        {
            var zoneRepository = _unitOfWork.GetRepository<Zone, int>();
            var zones = await zoneRepository.GetAllWithIncludeAsync(
                withTracking: false,
                include: query => query.Include(z => z.Warehouse)
            );
            return zones.ToList();
        }

        public async Task<IReadOnlyList<Zone>> GetZonesByWarehouseIdAsync(int warehouseId)
        {
            var zoneRepository = _unitOfWork.GetRepository<Zone, int>();
            var allZones = await zoneRepository.GetAllAsync(false);
            return allZones.Where(z => z.WarehouseId == warehouseId).ToList();
        }

        public async Task<Zone> UpdateZoneAsync(Zone zone)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(zone.Name))
                throw new ArgumentException("Zone Name is required.");
            if (zone.WarehouseId <= 0)
                throw new ArgumentException("Valid Warehouse must be selected.");

            var zoneRepository = _unitOfWork.GetRepository<Zone, int>();
            var existingZone = await zoneRepository.GetByIdAsync(zone.Id);
            
            if (existingZone == null)
                throw new KeyNotFoundException($"Zone with ID {zone.Id} not found.");

            existingZone.Name = zone.Name;
            existingZone.WarehouseId = zone.WarehouseId;
            
            zoneRepository.Update(existingZone);

            await _unitOfWork.CompleteAsync();

            return existingZone;
        }

        public async Task DeleteZoneAsync(int id)
        {
            var zoneRepository = _unitOfWork.GetRepository<Zone, int>();
            var zone = await zoneRepository.GetByIdAsync(id);
            
            if (zone == null)
                throw new KeyNotFoundException($"Zone with ID {id} not found.");

            var aisleRepository = _unitOfWork.GetRepository<Aisle, int>();
            var aisles = await aisleRepository.GetAllAsync(false);
            var hasAisles = aisles.Any(a => a.ZoneId == id);
            
            if (hasAisles)
                throw new InvalidOperationException("Cannot delete Zone because it has related Aisles. Please delete the Aisles first.");

            zoneRepository.Delete(zone);

            await _unitOfWork.CompleteAsync();
        }
    }
}

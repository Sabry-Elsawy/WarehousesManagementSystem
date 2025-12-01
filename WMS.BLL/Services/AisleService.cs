using WMS.BLL.Interfaces;
using WMS.DAL;
using WMS.DAL.UnitOfWork;

namespace WMS.BLL.Services
{
    public class AisleService : IAisleService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AisleService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Aisle> CreateAisleAsync(Aisle aisle)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(aisle.Name))
                throw new ArgumentException("Aisle Name is required.");
            if (aisle.ZoneId <= 0)
                throw new ArgumentException("Valid Zone must be selected.");

            var aisleRepository = _unitOfWork.GetRepository<Aisle, int>();
            
            await aisleRepository.AddAsync(aisle);
            
            await _unitOfWork.CompleteAsync();

            return aisle;
        }

        public async Task<Aisle?> GetAisleByIdAsync(int id)
        {
            var aisleRepository = _unitOfWork.GetRepository<Aisle, int>();
            return await aisleRepository.GetByIdAsync(id);
        }

        public async Task<IReadOnlyList<Aisle>> GetAllAislesAsync()
        {
            var aisleRepository = _unitOfWork.GetRepository<Aisle, int>();
            return await aisleRepository.GetAllAsync(false);
        }

        public async Task<IReadOnlyList<Aisle>> GetAislesByZoneIdAsync(int zoneId)
        {
            var aisleRepository = _unitOfWork.GetRepository<Aisle, int>();
            var allAisles = await aisleRepository.GetAllAsync(false);
            return allAisles.Where(a => a.ZoneId == zoneId).ToList();
        }

        public async Task<Aisle> UpdateAisleAsync(Aisle aisle)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(aisle.Name))
                throw new ArgumentException("Aisle Name is required.");
            if (aisle.ZoneId <= 0)
                throw new ArgumentException("Valid Zone must be selected.");

            var aisleRepository = _unitOfWork.GetRepository<Aisle, int>();
            var existingAisle = await aisleRepository.GetByIdAsync(aisle.Id);
            
            if (existingAisle == null)
                throw new KeyNotFoundException($"Aisle with ID {aisle.Id} not found.");

            // Update fields
            existingAisle.Name = aisle.Name;
            existingAisle.ZoneId = aisle.ZoneId;
            
            aisleRepository.Update(existingAisle);

            await _unitOfWork.CompleteAsync();

            return existingAisle;
        }

        public async Task DeleteAisleAsync(int id)
        {
            var aisleRepository = _unitOfWork.GetRepository<Aisle, int>();
            var aisle = await aisleRepository.GetByIdAsync(id);
            
            if (aisle == null)
                throw new KeyNotFoundException($"Aisle with ID {id} not found.");

            var rackRepository = _unitOfWork.GetRepository<Rack, int>();
            var racks = await rackRepository.GetAllAsync(false);
            var hasRacks = racks.Any(r => r.AisleId == id);
            
            if (hasRacks)
                throw new InvalidOperationException("Cannot delete Aisle because it has related Racks. Please delete the Racks first.");

            aisleRepository.Delete(aisle);

            await _unitOfWork.CompleteAsync();
        }
    }
}

using WMS.BLL.Interfaces;
using WMS.DAL;
using WMS.DAL.Entities;
using WMS.DAL.UnitOfWork;

namespace WMS.BLL.Services
{
    public class RackService : IRackService
    {
        private readonly IUnitOfWork _unitOfWork;

        public RackService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Rack> CreateRackAsync(Rack rack)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(rack.Name))
                throw new ArgumentException("Rack Name is required.");
            if (rack.AisleId <= 0)
                throw new ArgumentException("Valid Aisle must be selected.");

            var rackRepository = _unitOfWork.GetRepository<Rack, int>();
            
            await rackRepository.AddAsync(rack);
            
            await _unitOfWork.CompleteAsync();

            return rack;
        }

        public async Task<Rack?> GetRackByIdAsync(int id)
        {
            var rackRepository = _unitOfWork.GetRepository<Rack, int>();
            return await rackRepository.GetByIdAsync(id);
        }

        public async Task<IReadOnlyList<Rack>> GetAllRacksAsync()
        {
            var rackRepository = _unitOfWork.GetRepository<Rack, int>();
            return await rackRepository.GetAllAsync(false);
        }

        public async Task<IReadOnlyList<Rack>> GetRacksByAisleIdAsync(int aisleId)
        {
            var rackRepository = _unitOfWork.GetRepository<Rack, int>();
            var allRacks = await rackRepository.GetAllAsync(false);
            return allRacks.Where(r => r.AisleId == aisleId).ToList();
        }

        public async Task<Rack> UpdateRackAsync(Rack rack)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(rack.Name))
                throw new ArgumentException("Rack Name is required.");
            if (rack.AisleId <= 0)
                throw new ArgumentException("Valid Aisle must be selected.");

            var rackRepository = _unitOfWork.GetRepository<Rack, int>();
            var existingRack = await rackRepository.GetByIdAsync(rack.Id);
            
            if (existingRack == null)
                throw new KeyNotFoundException($"Rack with ID {rack.Id} not found.");

            existingRack.Name = rack.Name;
            existingRack.AisleId = rack.AisleId;
            
            rackRepository.Update(existingRack);

            await _unitOfWork.CompleteAsync();

            return existingRack;
        }

        public async Task DeleteRackAsync(int id)
        {
            var rackRepository = _unitOfWork.GetRepository<Rack, int>();
            var rack = await rackRepository.GetByIdAsync(id);
            
            if (rack == null)
                throw new KeyNotFoundException($"Rack with ID {id} not found.");

            var binRepository = _unitOfWork.GetRepository<Bin, int>();
            var bins = await binRepository.GetAllAsync(false);
            var hasBins = bins.Any(b => b.RackId == id);
            
            if (hasBins)
                throw new InvalidOperationException("Cannot delete Rack because it has related Bins. Please delete the Bins first.");

            rackRepository.Delete(rack);

            await _unitOfWork.CompleteAsync();
        }
    }
}

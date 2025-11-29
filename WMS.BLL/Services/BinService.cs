using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WMS.BLL.Interfaces;
using WMS.DAL.Entities;
using WMS.DAL.UnitOfWork;

namespace WMS.BLL.Services
{
    public class BinService : IBinService
    {
        private readonly IUnitOfWork _unitOfWork;

        public BinService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Bin> CreateBinAsync(Bin bin)
        {
            ValidateBin(bin);
            var repo = _unitOfWork.GetRepository<Bin, int>();
            await repo.AddAsync(bin);
            await _unitOfWork.CompleteAsync();
            return bin;
        }

        public async Task<Bin?> GetBinByIdAsync(int id)
        {
            var repo = _unitOfWork.GetRepository<Bin, int>();
            return await repo.GetByIdAsync(id);
        }

        public async Task<IReadOnlyList<Bin>> GetAllBinsAsync()
        {
            var repo = _unitOfWork.GetRepository<Bin, int>();
            return await repo.GetAllAsync(false); // Assuming false means no tracking or similar
        }

        public async Task<Bin> UpdateBinAsync(Bin bin)
        {
            ValidateBin(bin);
            var repo = _unitOfWork.GetRepository<Bin, int>();
            var existing = await repo.GetByIdAsync(bin.Id);
            if (existing == null)
                throw new ArgumentException("Bin not found.");

            existing.Code = bin.Code;
            existing.Capacity = bin.Capacity;
            existing.RackId = bin.RackId;
            existing.BinTypeId = bin.BinTypeId;

            repo.Update(existing);
            await _unitOfWork.CompleteAsync();
            return existing;
        }

        public async Task DeleteBinAsync(int id)
        {
            var repo = _unitOfWork.GetRepository<Bin, int>();
            var existing = await repo.GetByIdAsync(id);
            if (existing == null)
                throw new KeyNotFoundException($"Bin with ID {id} not found.");

            repo.Delete(existing);
            await _unitOfWork.CompleteAsync();
        }

        private void ValidateBin(Bin bin)
        {
            if (string.IsNullOrWhiteSpace(bin.Code))
                throw new ArgumentException("Bin Code is required.");
            if (bin.RackId <= 0)
                throw new ArgumentException("Valid Rack must be selected.");
            if (bin.BinTypeId <= 0)
                throw new ArgumentException("Valid Bin Type must be selected.");
        }
    }
}
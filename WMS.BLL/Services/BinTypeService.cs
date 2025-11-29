using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WMS.BLL.Interfaces;
using WMS.DAL.Entities;
using WMS.DAL.UnitOfWork;

namespace WMS.BLL.Services
{
    public class BinTypeService : IBinTypeService
    {
        private readonly IUnitOfWork _unitOfWork;

        public BinTypeService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<BinType> CreateBinTypeAsync(BinType binType)
        {
            if (string.IsNullOrWhiteSpace(binType.Name))
                throw new ArgumentException("Bin Type Name is required.");

            var repo = _unitOfWork.GetRepository<BinType, int>();
            await repo.AddAsync(binType);
            await _unitOfWork.CompleteAsync();
            return binType;
        }

        public async Task<BinType?> GetBinTypeByIdAsync(int id)
        {
            var repo = _unitOfWork.GetRepository<BinType, int>();
            return await repo.GetByIdAsync(id);
        }

        public async Task<IReadOnlyList<BinType>> GetAllBinTypesAsync()
        {
            var repo = _unitOfWork.GetRepository<BinType, int>();
            return await repo.GetAllAsync(false);
        }

        public async Task<BinType> UpdateBinTypeAsync(BinType binType)
        {
            if (string.IsNullOrWhiteSpace(binType.Name))
                throw new ArgumentException("Bin Type Name is required.");

            var repo = _unitOfWork.GetRepository<BinType, int>();
            var existing = await repo.GetByIdAsync(binType.Id);
            if (existing == null)
                throw new ArgumentException("Bin Type not found.");

            existing.Name = binType.Name;
            existing.Description = binType.Description;

            repo.Update(existing);
            await _unitOfWork.CompleteAsync();
            return existing;
        }

        public async Task DeleteBinTypeAsync(int id)
        {
            var repo = _unitOfWork.GetRepository<BinType, int>();
            var existing = await repo.GetByIdAsync(id);
            if (existing == null)
                throw new KeyNotFoundException($"Bin Type with ID {id} not found.");

            repo.Delete(existing);
            await _unitOfWork.CompleteAsync();
        }
    }
}
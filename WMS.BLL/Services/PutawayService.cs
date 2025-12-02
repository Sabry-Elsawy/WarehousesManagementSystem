using Microsoft.EntityFrameworkCore;
using WMS.BLL.DTOs;
using WMS.BLL.Interfaces;
using WMS.DAL;
using WMS.DAL.Entities;
using WMS.DAL.UnitOfWork;

namespace WMS.BLL.Services
{
    public class PutawayService : IPutawayService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PutawayService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Putaway>> GetAllAsync()
        {
            var repo = _unitOfWork.GetRepository<Putaway, int>();

            return await repo.GetAllWithIncludeAsync(
                withTracking: false,
                include: q => q
                    .Include(p => p.ReceiptItem)
                    .ThenInclude(ri => ri.Product)
            );
        }

        public async Task<Putaway?> GetByIdAsync(int id)
        {
            var repo = _unitOfWork.GetRepository<Putaway, int>();
            return await repo.GetByIdAsync(id,
                    include: q => q
                        .Include(p => p.ReceiptItem)
                        .ThenInclude(r => r.Product)
    );
        }

        public async Task<IEnumerable<BinDTO>> GetAvailableBinsAsync(int putawayId)
        {
            var putaway = await _unitOfWork.GetRepository<Putaway, int>()
                .GetByIdAsync(putawayId);

            if (putaway == null)
                return Enumerable.Empty<BinDTO>();

            var bins = await _unitOfWork.GetRepository<Bin, int>()
                .GetAllWithIncludeAsync(false, q => q.Include(b => b.Inventories));

            return bins.Select(b =>
            {
                var used = b.Inventories.Sum(i => i.Quantity);

                return new BinDTO
                {
                    BinId = b.Id,
                    Code = b.Code,
                    Capacity = b.Capacity,
                    Used = used
                };
            }).ToList();
        }


        public async Task<bool> AssignAsync(int putawayId, int binId, int qty)
        {
            var putawayRepo = _unitOfWork.GetRepository<Putaway, int>();
            var putaway = await putawayRepo.GetByIdAsync(putawayId);

            if (putaway == null)
                throw new InvalidOperationException("Putaway not found");

            var binRepo = _unitOfWork.GetRepository<Bin, int>();
            var bin = await binRepo.GetByIdAsync(binId);

            if (bin == null)
                throw new InvalidOperationException("Bin not found");

            var inventoryRepo = _unitOfWork.GetRepository<Inventory, int>();
            var inventoryItems = await inventoryRepo.GetAllAsync();
            var used = inventoryItems.Where(i => i.BinId == binId).Sum(i => i.Quantity);

            if (bin.Capacity - used < qty)
                throw new InvalidOperationException("Not enough bin capacity");

            var putawayBinRepo = _unitOfWork.GetRepository<PutawayBin, int>();
            var entry = new PutawayBin
            {
                PutawayId = putawayId,
                BinId = binId,
                Qty = qty
            };

            await putawayBinRepo.AddAsync(entry);
            await _unitOfWork.CompleteAsync();
            return true;
        }

        public async Task<bool> CompleteAsync(int putawayId)
        {
            var putawayRepo = _unitOfWork.GetRepository<Putaway, int>();
            var putaway = await putawayRepo.GetByIdAsync(putawayId);

            if (putaway == null)
                throw new InvalidOperationException("Putaway not found");

            var linksRepo = _unitOfWork.GetRepository<PutawayBin, int>();
            var links = await linksRepo.GetAllWithIncludeAsync(
                withTracking: false,
                include: q => q.Where(l => l.PutawayId == putawayId)
            );


            if (!links.Any())
                throw new InvalidOperationException("No bins assigned");

            var receiptItemRepo = _unitOfWork.GetRepository<ReceiptItem, int>();
            var receiptItem = await receiptItemRepo.GetByIdAsync(putaway.ReceiptItemId);

            if (receiptItem == null)
                throw new InvalidOperationException("Invalid receipt item");

            var inventoryRepo = _unitOfWork.GetRepository<Inventory, int>();

            foreach (var link in links)
            {
                var inv = new Inventory
                {
                    ProductId = receiptItem.ProductId,
                    BinId = link.BinId,
                    Quantity = link.Qty,
                    Status = "Available",
                    BatchNumber = "BATCH-001",
                    ExpiryDate = ""
                };

                await inventoryRepo.AddAsync(inv);
            }

            putaway.Status = PutawayStatus.Completed;
            putawayRepo.Update(putaway);

            await _unitOfWork.CompleteAsync();
            return true;
        }
    }
}

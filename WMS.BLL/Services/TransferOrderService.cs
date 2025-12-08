using Microsoft.EntityFrameworkCore;
using WMS.BLL.Interfaces;
using WMS.DAL;
using WMS.DAL.Entities;
using WMS.DAL.UnitOfWork;

namespace WMS.BLL.Services;

public class TransferOrderService : ITransferOrderService
{
    private readonly IUnitOfWork _unitOfWork;

    public TransferOrderService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyList<TransferOrder>> GetAllAsync()
    {
        var repository = _unitOfWork.GetRepository<TransferOrder, int>();
        var orders = await repository.GetAllWithIncludeAsync(false, q => q
            .Include(t => t.SourceWarehouse)
            .Include(t => t.DestinationWarehouse)
            .Include(t => t.TransferOrderItems)
            .ThenInclude(i => i.Product));
        return orders.OrderByDescending(o => o.CreatedOn).ToList();
    }

    public async Task<TransferOrder?> GetByIdAsync(int id)
    {
        var repository = _unitOfWork.GetRepository<TransferOrder, int>();
        return await repository.GetByIdAsync(id, q => q
            .Include(t => t.SourceWarehouse)
            .Include(t => t.DestinationWarehouse)
            .Include(t => t.TransferOrderItems)
            .ThenInclude(i => i.Product));
    }

    public async Task CreateAsync(TransferOrder transferOrder)
    {
        var repository = _unitOfWork.GetRepository<TransferOrder, int>();
        transferOrder.Status = TransferOrderStatus.Pending;
        await repository.AddAsync(transferOrder);
        await _unitOfWork.CompleteAsync();
    }

    public async Task AddItemAsync(int transferOrderId, int productId, int quantity)
    {
        var orderRepo = _unitOfWork.GetRepository<TransferOrder, int>();
        var itemRepo = _unitOfWork.GetRepository<TransferOrderItem, int>();
        var productRepo = _unitOfWork.GetRepository<Product, int>();

        var order = await orderRepo.GetByIdAsync(transferOrderId, q => q.Include(t => t.TransferOrderItems));
        if (order == null) throw new ArgumentException("Transfer Order not found");
        
        if (order.Status != TransferOrderStatus.Pending)
            throw new InvalidOperationException("Can only add items to Pending orders");

        var product = await productRepo.GetByIdAsync(productId);
        if (product == null) throw new ArgumentException("Product not found");

        var existingItem = order.TransferOrderItems.FirstOrDefault(i => i.ProductId == productId);
        if (existingItem != null)
        {
            existingItem.Qty += quantity;
            itemRepo.Update(existingItem);
        }
        else
        {
            var newItem = new TransferOrderItem
            {
                TransferOrderId = transferOrderId,
                ProductId = productId,
                Qty = quantity
            };
            await itemRepo.AddAsync(newItem);
        }
        
        await _unitOfWork.CompleteAsync();
    }

    public async Task ApproveAsync(int id)
    {
        var repository = _unitOfWork.GetRepository<TransferOrder, int>();
        var order = await repository.GetByIdAsync(id);
        if (order != null && order.Status == TransferOrderStatus.Pending)
        {
            order.Status = TransferOrderStatus.Approved;
            repository.Update(order);
            await _unitOfWork.CompleteAsync();
        }
    }

    public async Task IssueAsync(int id)
    {
        var repository = _unitOfWork.GetRepository<TransferOrder, int>();
        var inventoryRepo = _unitOfWork.GetRepository<Inventory, int>();
        var transRepo = _unitOfWork.GetRepository<InventoryTransaction, int>();

        var order = await repository.GetByIdAsync(id, q => q.Include(t => t.TransferOrderItems));
        if (order == null) throw new ArgumentException("Order not found");

        if (order.Status != TransferOrderStatus.Approved && order.Status != TransferOrderStatus.Pending)
            throw new InvalidOperationException($"Cannot issue order with status {order.Status}. Must be Pending or Approved.");

        // Deduct inventory from Source Warehouse
        foreach (var item in order.TransferOrderItems)
        {
            var qtyRemaining = item.Qty;

            // Find stock in source warehouse (FIFO: order by Bin ID or Expiry)
            var allInventory = await inventoryRepo.GetAllWithIncludeAsync(true, q => q
                .Include(i => i.Bin)
                .ThenInclude(b => b.Rack)
                .ThenInclude(r => r.Aisle)
                .ThenInclude(a => a.Zone));
            
            var sourceInventory = allInventory
                .Where(i => i.Bin?.Rack?.Aisle?.Zone?.WarehouseId == order.SourceWarehouseId && i.ProductId == item.ProductId && (i.Quantity - i.ReservedQuantity) > 0)
                .OrderBy(i => i.BinId) // Simple FIFO strategy
                .ToList();

            if (sourceInventory.Sum(i => i.Quantity - i.ReservedQuantity) < item.Qty)
            {
                throw new InvalidOperationException($"Insufficient stock for Product {item.ProductId} in Source Warehouse");
            }

            foreach (var inv in sourceInventory)
            {
                if (qtyRemaining <= 0) break;

                var available = inv.Quantity - inv.ReservedQuantity;
                var toTake = Math.Min(available, qtyRemaining);

                inv.Quantity -= toTake;
                inventoryRepo.Update(inv);
                qtyRemaining -= toTake;

                // Log Transaction
                await transRepo.AddAsync(new InventoryTransaction
                {
                    TransactionType = "Transfer Out",
                    QuantityChange = -toTake,
                    ProductId = item.ProductId,
                    SourceBinId = inv.BinId,
                    ReferenceNumber = $"TO-{order.Id}",
                    Reason = $"Transfer to Warehouse {order.DestinationWarehouseId}"
                });
            }
        }

        order.Status = TransferOrderStatus.Issued;
        repository.Update(order);
        await _unitOfWork.CompleteAsync();
    }

    public async Task ReceiveAsync(int id)
    {
        var repository = _unitOfWork.GetRepository<TransferOrder, int>();
        var inventoryRepo = _unitOfWork.GetRepository<Inventory, int>();
        var binRepo = _unitOfWork.GetRepository<Bin, int>();
        var transRepo = _unitOfWork.GetRepository<InventoryTransaction, int>();

        var order = await repository.GetByIdAsync(id, q => q.Include(t => t.TransferOrderItems));
        if (order == null) throw new ArgumentException("Order not found");

        if (order.Status != TransferOrderStatus.Issued)
            throw new InvalidOperationException("Can only receive Issued orders");

        // Add inventory to Destination Warehouse
        // Find a receiving bin in destination warehouse (first available bin for now)
        var allBins = await binRepo.GetAllWithIncludeAsync(false, q => q
            .Include(b => b.Rack)
            .ThenInclude(r => r.Aisle)
            .ThenInclude(a => a.Zone));
            
        var destBin = allBins.FirstOrDefault(b => b.Rack?.Aisle?.Zone?.WarehouseId == order.DestinationWarehouseId);

        if (destBin == null)
            throw new InvalidOperationException($"No bins found in Destination Warehouse {order.DestinationWarehouseId}");

        foreach (var item in order.TransferOrderItems)
        {
            // Check if inventory record exists for this product in dest bin
            var allInventory = await inventoryRepo.GetAllAsync(true);
            var destInv = allInventory.FirstOrDefault(i => i.BinId == destBin.Id && i.ProductId == item.ProductId);

            if (destInv != null)
            {
                destInv.Quantity += item.Qty;
                inventoryRepo.Update(destInv);
            }
            else
            {
                await inventoryRepo.AddAsync(new Inventory
                {
                    ProductId = item.ProductId,
                    BinId = destBin.Id,
                    Quantity = item.Qty,
                    Status = "Available",
                    BatchNumber = "TRANSFER",
                    ExpiryDate = ""
                });
            }

            // Log Transaction
            await transRepo.AddAsync(new InventoryTransaction
            {
                TransactionType = "Transfer In",
                QuantityChange = item.Qty,
                ProductId = item.ProductId,
                DestinationBinId = destBin.Id,
                ReferenceNumber = $"TO-{order.Id}",
                Reason = $"Transfer from Warehouse {order.SourceWarehouseId}"
            });
        }

        order.Status = TransferOrderStatus.Received;
        repository.Update(order);
        await _unitOfWork.CompleteAsync();
    }

    public async Task CancelAsync(int id)
    {
        var repository = _unitOfWork.GetRepository<TransferOrder, int>();
        var order = await repository.GetByIdAsync(id);
        
        if (order == null) throw new ArgumentException("Order not found");
        
        if (order.Status == TransferOrderStatus.Received || order.Status == TransferOrderStatus.Issued)
            throw new InvalidOperationException("Cannot cancel orders that are Issued or Received");

        order.Status = TransferOrderStatus.Cancelled;
        repository.Update(order);
        await _unitOfWork.CompleteAsync();
    }
}

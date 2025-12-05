using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using WMS.BLL.Services;
using WMS.BLL.DTOs;
using WMS.DAL;
using WMS.DAL.Entities;
using WMS.DAL.UnitOfWork;
using WMS.DAL.Contract;
using Microsoft.EntityFrameworkCore;

namespace WMS.Tests.Services;

/// <summary>
/// Comprehensive unit tests for PutawayService covering all business logic
/// </summary>
public class PutawayServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IGenericRepository<Putaway, int>> _mockPutawayRepo;
    private readonly Mock<IGenericRepository<PutawayBin, int>> _mockPutawayBinRepo;
    private readonly Mock<IGenericRepository<Bin, int>> _mockBinRepo;
    private readonly Mock<IGenericRepository<Inventory, int>> _mockInventoryRepo;
    private readonly Mock<IGenericRepository<ReceiptItem, int>> _mockReceiptItemRepo;
    private readonly Mock<ILogger<PutawayService>> _mockLogger;
    private readonly PutawayService _service;

    public PutawayServiceTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockPutawayRepo = new Mock<IGenericRepository<Putaway, int>>();
        _mockPutawayBinRepo = new Mock<IGenericRepository<PutawayBin, int>>();
        _mockBinRepo = new Mock<IGenericRepository<Bin, int>>();
        _mockInventoryRepo = new Mock<IGenericRepository<Inventory, int>>();
        _mockReceiptItemRepo = new Mock<IGenericRepository<ReceiptItem, int>>();
        _mockLogger = new Mock<ILogger<PutawayService>>();

        _mockUnitOfWork.Setup(uow => uow.GetRepository<Putaway, int>()).Returns(_mockPutawayRepo.Object);
        _mockUnitOfWork.Setup(uow => uow.GetRepository<PutawayBin, int>()).Returns(_mockPutawayBinRepo.Object);
        _mockUnitOfWork.Setup(uow => uow.GetRepository<Bin, int>()).Returns(_mockBinRepo.Object);
        _mockUnitOfWork.Setup(uow => uow.GetRepository<Inventory, int>()).Returns(_mockInventoryRepo.Object);
        _mockUnitOfWork.Setup(uow => uow.GetRepository<ReceiptItem, int>()).Returns(_mockReceiptItemRepo.Object);

        _service = new PutawayService(_mockUnitOfWork.Object, _mockLogger.Object);
    }

    #region CreatePutawayForReceiptItemAsync Tests

    [Fact]
    public async Task CreatePutawayForReceiptItemAsync_WithValidReceiptItem_CreatesPutaway()
    {
        // Arrange
        var receiptItemId = 1;
        var qty = 50;
        var createdBy = "testuser";

        var receiptItem = new ReceiptItem
        {
            Id = receiptItemId,
            ProductId = 100,
            QtyReceived = 100,
            SKU = "TEST-SKU",
            Product = new Product { Id = 100, Name = "Test Product" }
        };

        _mockReceiptItemRepo.Setup(r => r.GetByIdAsync(receiptItemId, It.IsAny<Func<IQueryable<ReceiptItem>, IQueryable<ReceiptItem>>>()))
            .ReturnsAsync(receiptItem);

        _mockPutawayRepo.Setup(r => r.GetAllWithIncludeAsync(false, It.IsAny<Func<IQueryable<Putaway>, IQueryable<Putaway>>>()))
            .ReturnsAsync(new List<Putaway>());

        Putaway? capturedPutaway = null;
        _mockPutawayRepo.Setup(r => r.AddAsync(It.IsAny<Putaway>()))
            .Callback<Putaway>(p => capturedPutaway = p)
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.CreatePutawayForReceiptItemAsync(receiptItemId, qty, createdBy);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(qty, result.Qty);
        Assert.Equal(receiptItem.SKU, result.SKU);
        Assert.Equal(receiptItem.Product.Name, result.ProductName);
        Assert.NotNull(capturedPutaway);
        Assert.Equal(PutawayStatus.Pending, capturedPutaway.Status);
        Assert.Equal(createdBy, capturedPutaway.CreatedBy);
        _mockPutawayRepo.Verify(r => r.AddAsync(It.IsAny<Putaway>()), Times.Once);
        _mockUnitOfWork.Verify(uow => uow.CompleteAsync(), Times.Once);
    }

    [Fact]
    public async Task CreatePutawayForReceiptItemAsync_WithInvalidReceiptItem_ThrowsException()
    {
        // Arrange
        var receiptItemId = 999;
        
        _mockReceiptItemRepo.Setup(r => r.GetByIdAsync(receiptItemId, It.IsAny<Func<IQueryable<ReceiptItem>, IQueryable<ReceiptItem>>>()))
            .ReturnsAsync((ReceiptItem?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => _service.CreatePutawayForReceiptItemAsync(receiptItemId, 50, "testuser")
        );

        Assert.Contains("not found", exception.Message);
        _mockPutawayRepo.Verify(r => r.AddAsync(It.IsAny<Putaway>()), Times.Never);
    }

    [Fact]
    public async Task CreatePutawayForReceiptItemAsync_WithExcessiveQty_ThrowsException()
    {
        // Arrange
        var receiptItemId = 1;
        var receiptItem = new ReceiptItem
        {
            Id = receiptItemId,
            ProductId = 100,
            QtyReceived = 100,
            SKU = "TEST-SKU",
            Product = new Product { Id = 100, Name = "Test Product" }
        };

        _mockReceiptItemRepo.Setup(r => r.GetByIdAsync(receiptItemId, It.IsAny<Func<IQueryable<ReceiptItem>, IQueryable<ReceiptItem>>>()))
            .ReturnsAsync(receiptItem);

        // Existing putaway already assigned 80 units
        var existingPutaways = new List<Putaway> { new Putaway { Qty = 80 } };
        _mockPutawayRepo.Setup(r => r.GetAllWithIncludeAsync(false, It.IsAny<Func<IQueryable<Putaway>, IQueryable<Putaway>>>()))
            .ReturnsAsync(existingPutaways);

        // Try to create putaway for 30 units (available is only 20)
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => _service.CreatePutawayForReceiptItemAsync(receiptItemId, 30, "testuser")
        );

        Assert.Contains("exceeds available quantity", exception.Message);
    }

    #endregion

    #region AutoAssignBinsAsync Tests

    [Fact]
    public async Task AutoAssignBinsAsync_WithSufficientCapacity_ReturnsFullAssignment()
    {
        // Arrange
        var putawayId = 1;
        var putaway = new Putaway
        {
            Id = putawayId,
            Qty = 100,
            Status = PutawayStatus.Pending,
            ReceiptItem = new ReceiptItem
            {
                ProductId = 100,
                Product = new Product { Id = 100, Name = "Test Product" }
            }
        };

        var bins = new List<Bin>
        {
            new Bin { Id = 1, Code = "BIN-001", Capacity = 150, BinType = new BinType { Name = "Standard" }, Inventories = new List<Inventory>(), Rack = new Rack { Aisle = new Aisle { Zone = new Zone { Id = 1, Name = "Zone A" } } } }
        };

        _mockPutawayRepo.Setup(r => r.GetByIdAsync(putawayId, It.IsAny<Func<IQueryable<Putaway>, IQueryable<Putaway>>>()))
            .ReturnsAsync(putaway);

        _mockBinRepo.Setup(r => r.GetAllWithIncludeAsync(false, It.IsAny<Func<IQueryable<Bin>, IQueryable<Bin>>>()))
            .ReturnsAsync(bins);

        // Act
        var result = await _service.AutoAssignBinsAsync(putawayId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(100, result.TotalQty);
        Assert.Equal(100, result.AssignedQty);
        Assert.True(result.FullyAssigned);
        Assert.Single(result.SuggestedBins);
        Assert.Equal(100, result.SuggestedBins[0].Qty);
        _mockPutawayRepo.Verify(r => r.Update(It.Is<Putaway>(p => p.Status == PutawayStatus.Assigned)), Times.Once);
        _mockUnitOfWork.Verify(uow => uow.CompleteAsync(), Times.Once);
    }

    [Fact]
    public async Task AutoAssignBinsAsync_WithMultipleBinsNeeded_SplitsAcrossBins()
    {
        // Arrange
        var putawayId = 1;
        var putaway = new Putaway
        {
            Id = putawayId,
            Qty = 200,
            Status = PutawayStatus.Pending,
            ReceiptItem = new ReceiptItem
            {
                ProductId = 100,
                Product = new Product { Id = 100, Name = "Test Product" }
            }
        };

        var bins = new List<Bin>
        {
            new Bin { Id = 1, Code = "BIN-001", Capacity = 80, BinType = new BinType { Name = "Standard" }, Inventories = new List<Inventory>(), Rack = new Rack { Aisle = new Aisle { Zone = new Zone { Id = 1, Name = "Zone A" } } } },
            new Bin { Id = 2, Code = "BIN-002", Capacity = 80, BinType = new BinType { Name = "Standard" }, Inventories = new List<Inventory>(), Rack = new Rack { Aisle = new Aisle { Zone = new Zone { Id = 1, Name = "Zone A" } } } },
            new Bin { Id = 3, Code = "BIN-003", Capacity = 50, BinType = new BinType { Name = "Standard" }, Inventories = new List<Inventory>(), Rack = new Rack { Aisle = new Aisle { Zone = new Zone { Id = 1, Name = "Zone A" } } } }
        };

        _mockPutawayRepo.Setup(r => r.GetByIdAsync(putawayId, It.IsAny<Func<IQueryable<Putaway>, IQueryable<Putaway>>>()))
            .ReturnsAsync(putaway);

        _mockBinRepo.Setup(r => r.GetAllWithIncludeAsync(false, It.IsAny<Func<IQueryable<Bin>, IQueryable<Bin>>>()))
            .ReturnsAsync(bins);

        // Act
        var result = await _service.AutoAssignBinsAsync(putawayId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.AssignedQty);
        Assert.True(result.FullyAssigned);
        Assert.Equal(3, result.SuggestedBins.Count);
        // Greedy algorithm fills bins in order
        Assert.Equal(80, result.SuggestedBins[0].Qty);
        Assert.Equal(80, result.SuggestedBins[1].Qty);
        Assert.Equal(40, result.SuggestedBins[2].Qty);
    }

    [Fact]
    public async Task AutoAssignBinsAsync_WithInsufficientCapacity_ReturnsPartialAssignment()
    {
        // Arrange
        var putawayId = 1;
        var putaway = new Putaway
        {
            Id = putawayId,
            Qty = 200,
            Status = PutawayStatus.Pending,
            ReceiptItem = new ReceiptItem
            {
                ProductId = 100,
                Product = new Product { Id = 100, Name = "Test Product" }
            }
        };

        var bins = new List<Bin>
        {
            new Bin { Id = 1, Code = "BIN-001", Capacity = 50, BinType = new BinType { Name = "Standard" }, Inventories = new List<Inventory>(), Rack = new Rack { Aisle = new Aisle { Zone = new Zone { Id = 1, Name = "Zone A" } } } }
        };

        _mockPutawayRepo.Setup(r => r.GetByIdAsync(putawayId, It.IsAny<Func<IQueryable<Putaway>, IQueryable<Putaway>>>()))
            .ReturnsAsync(putaway);

        _mockBinRepo.Setup(r => r.GetAllWithIncludeAsync(false, It.IsAny<Func<IQueryable<Bin>, IQueryable<Bin>>>()))
            .ReturnsAsync(bins);

        // Act
        var result = await_service.AutoAssignBinsAsync(putawayId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.TotalQty);
        Assert.Equal(50, result.AssignedQty);
        Assert.False(result.FullyAssigned);
        Assert.NotNull(result.WarningMessage);
        Assert.Contains("150 units remaining", result.WarningMessage);
        // Should NOT update status to Assigned if not fully assigned
        _mockPutawayRepo.Verify(r => r.Update(It.IsAny<Putaway>()), Times.Never);
    }

    [Fact]
    public async Task AutoAssignBinsAsync_PrefersSameProductCoLocation()
    {
        // Arrange
        var putawayId = 1;
        var productId = 100;
        var putaway = new Putaway
        {
            Id = putawayId,
            Qty = 50,
            Status = PutawayStatus.Pending,
            ReceiptItem = new ReceiptItem
            {
                ProductId = productId,
                Product = new Product { Id = productId, Name = "Test Product" }
            }
        };

        var bins = new List<Bin>
        {
            new Bin
            {
                Id = 1,
                Code = "BIN-001",
                Capacity = 100,
                BinType = new BinType { Name = "Standard" },
                Inventories = new List<Inventory>(), // Empty bin
                Rack = new Rack { Aisle = new Aisle { Zone = new Zone { Id = 1, Name = "Zone A" } } }
            },
            new Bin
            {
                Id = 2,
                Code = "BIN-002",
                Capacity = 100,
                BinType = new BinType { Name = "Standard" },
                Inventories = new List<Inventory> { new Inventory { ProductId = productId, Quantity = 30 } }, // Has same product
                Rack = new Rack { Aisle = new Aisle { Zone = new Zone { Id = 1, Name = "Zone A" } } }
            }
        };

        _mockPutawayRepo.Setup(r => r.GetByIdAsync(putawayId, It.IsAny<Func<IQueryable<Putaway>, IQueryable<Putaway>>>()))
            .ReturnsAsync(putaway);

        _mockBinRepo.Setup(r => r.GetAllWithIncludeAsync(false, It.IsAny<Func<IQueryable<Bin>, IQueryable<Bin>>>()))
            .ReturnsAsync(bins);

        // Act
        var result = await _service.AutoAssignBinsAsync(putawayId);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.SuggestedBins);
        // Should prefer BIN-002 because it already has the same product (co-location)
        Assert.Equal("BIN-002", result.SuggestedBins[0].BinCode);
    }

    #endregion

    #region AssignBinsManualAsync Tests

    [Fact]
    public async Task AssignBinsManualAsync_WithValidAssignments_SavesAssignments()
    {
        // Arrange
        var putawayId = 1;
        var putaway = new Putaway { Id = putawayId, Qty = 100, Status = PutawayStatus.Pending };
        var assignments = new List<PutawayBinDto>
        {
            new PutawayBinDto { BinId = 1, Qty = 60 },
            new PutawayBinDto { BinId = 2, Qty = 40 }
        };

        var bin1 = new Bin { Id = 1, Code = "BIN-001", Capacity = 100 };
        var bin2 = new Bin { Id = 2, Code = "BIN-002", Capacity = 100 };

        _mockPutawayRepo.Setup(r => r.GetByIdAsync(putawayId)).ReturnsAsync(putaway);
        _mockBinRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(bin1);
        _mockBinRepo.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(bin2);
        _mockInventoryRepo.Setup(r => r.GetAllWithIncludeAsync(false, It.IsAny<Func<IQueryable<Inventory>, IQueryable<Inventory>>>()))
            .ReturnsAsync(new List<Inventory>());
        _mockPutawayBinRepo.Setup(r => r.GetAllWithIncludeAsync(true, It.IsAny<Func<IQueryable<PutawayBin>, IQueryable<PutawayBin>>>()))
            .ReturnsAsync(new List<PutawayBin>());

        // Act
        await _service.AssignBinsManualAsync(putawayId, assignments, "testuser");

        // Assert
        _mockPutawayBinRepo.Verify(r => r.AddAsync(It.IsAny<PutawayBin>()), Times.Exactly(2));
        _mockPutawayRepo.Verify(r => r.Update(It.Is<Putaway>(p => p.Status == PutawayStatus.Assigned)), Times.Once);
        _mockUnitOfWork.Verify(uow => uow.CompleteAsync(), Times.Once);
    }

    [Fact]
    public async Task AssignBinsManualAsync_WithTotalQtyMismatch_ThrowsException()
    {
        // Arrange
        var putawayId = 1;
        var putaway = new Putaway { Id = putawayId, Qty = 100, Status = PutawayStatus.Pending };
        var assignments = new List<PutawayBinDto>
        {
            new PutawayBinDto { BinId = 1, Qty = 50 } // Only 50, not 100
        };

        _mockPutawayRepo.Setup(r => r.GetByIdAsync(putawayId)).ReturnsAsync(putaway);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => _service.AssignBinsManualAsync(putawayId, assignments, "testuser")
        );

        Assert.Contains("does not match putaway quantity", exception.Message);
        _mockPutawayBinRepo.Verify(r => r.AddAsync(It.IsAny<PutawayBin>()), Times.Never);
    }

    [Fact]
    public async Task AssignBinsManualAsync_ExceedingCapacity_ThrowsException()
    {
        // Arrange
        var putawayId = 1;
        var putaway = new Putaway { Id = putawayId, Qty = 100, Status = PutawayStatus.Pending };
        var bin = new Bin { Id = 1, Code = "BIN-001", Capacity = 50 };
        var assignments = new List<PutawayBinDto>
        {
            new PutawayBinDto { BinId = 1, Qty = 100 } // Exceeds bin capacity of 50
        };

        _mockPutawayRepo.Setup(r => r.GetByIdAsync(putawayId)).ReturnsAsync(putaway);
        _mockBinRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(bin);
        _mockInventoryRepo.Setup(r => r.GetAllWithIncludeAsync(false, It.IsAny<Func<IQueryable<Inventory>, IQueryable<Inventory>>>()))
            .ReturnsAsync(new List<Inventory>());

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => _service.AssignBinsManualAsync(putawayId, assignments, "testuser")
        );

        Assert.Contains("does not have sufficient capacity", exception.Message);
    }

    #endregion

    #region ExecutePutawayAsync Tests

    [Fact]
    public async Task ExecutePutawayAsync_CreatesNewInventory_WhenNotExists()
    {
        // Arrange
        var putawayId = 1;
        var productId = 100;
        var binId = 1;
        var qty = 50;

        var putaway = new Putaway
        {
            Id = putawayId,
            Status = PutawayStatus.Assigned,
            ReceiptItem = new ReceiptItem
            {
                ProductId = productId,
                ASNItem = new AdvancedShippingNoticeItem { LotNumber = "LOT123", ExpiryDate = DateTime.Today.AddMonths(12) }
            },
            PutawayBins = new List<PutawayBin>
            {
                new PutawayBin { PutawayId = putawayId, BinId = binId, Qty = qty }
            }
        };

        var bin = new Bin { Id = binId, Code = "BIN-001", Capacity = 100 };

        _mockPutawayRepo.Setup(r => r.GetByIdAsync(putawayId, It.IsAny<Func<IQueryable<Putaway>, IQueryable<Putaway>>>()))
            .ReturnsAsync(putaway);
        _mockBinRepo.Setup(r => r.GetByIdAsync(binId)).ReturnsAsync(bin);
        _mockInventoryRepo.Setup(r => r.GetAllWithIncludeAsync(false, It.IsAny<Func<IQueryable<Inventory>, IQueryable<Inventory>>>()))
            .ReturnsAsync(new List<Inventory>()); // No existing inventory

        Inventory? capturedInventory = null;
        _mockInventoryRepo.Setup(r => r.AddAsync(It.IsAny<Inventory>()))
            .Callback<Inventory>(i => capturedInventory = i)
            .Returns(Task.CompletedTask);

        // Act
        await _service.ExecutePutawayAsync(putawayId, "testuser");

        // Assert
        _mockInventoryRepo.Verify(r => r.AddAsync(It.IsAny<Inventory>()), Times.Once);
        Assert.NotNull(capturedInventory);
        Assert.Equal(productId, capturedInventory.ProductId);
        Assert.Equal(binId, capturedInventory.BinId);
        Assert.Equal(qty, capturedInventory.Quantity);
        Assert.Equal("LOT123", capturedInventory.BatchNumber);
        _mockPutawayRepo.Verify(r => r.Update(It.Is<Putaway>(p => p.Status == PutawayStatus.Completed && p.PerformedBy == "testuser")), Times.Once);
    }

    [Fact]
    public async Task ExecutePutawayAsync_IncrementsInventory_WhenExists()
    {
        // Arrange
        var putawayId = 1;
        var productId = 100;
        var binId = 1;
        var qty = 50;

        var existingInventory = new Inventory { Id = 1, ProductId = productId, BinId = binId, Quantity = 30 };

        var putaway = new Putaway
        {
            Id = putawayId,
            Status = PutawayStatus.Assigned,
            ReceiptItem = new ReceiptItem { ProductId = productId, ASNItem = new AdvancedShippingNoticeItem() },
            PutawayBins = new List<PutawayBin>
            {
                new PutawayBin { PutawayId = putawayId, BinId = binId, Qty = qty }
            }
        };

        var bin = new Bin { Id = binId, Code = "BIN-001", Capacity = 200 };

        _mockPutawayRepo.Setup(r => r.GetByIdAsync(putawayId, It.IsAny<Func<IQueryable<Putaway>, IQueryable<Putaway>>>()))
            .ReturnsAsync(putaway);
        _mockBinRepo.Setup(r => r.GetByIdAsync(binId)).ReturnsAsync(bin);
        _mockInventoryRepo.Setup(r => r.GetAllWithIncludeAsync(false, It.IsAny<Func<IQueryable<Inventory>, IQueryable<Inventory>>>()))
            .ReturnsAsync(new List<Inventory> { existingInventory });

        // Act
        await _service.ExecutePutawayAsync(putawayId, "testuser");

        // Assert
        Assert.Equal(80, existingInventory.Quantity); // 30 + 50
        _mockInventoryRepo.Verify(r => r.Update(existingInventory), Times.Once);
        _mockInventoryRepo.Verify(r => r.AddAsync(It.IsAny<Inventory>()), Times.Never);
    }

    [Fact]
    public async Task ExecutePutawayAsync_WithWrongStatus_ThrowsException()
    {
        // Arrange
        var putawayId = 1;
        var putaway = new Putaway { Id = putawayId, Status = PutawayStatus.Pending };

        _mockPutawayRepo.Setup(r => r.GetByIdAsync(putawayId, It.IsAny<Func<IQueryable<Putaway>, IQueryable<Putaway>>>()))
            .ReturnsAsync(putaway);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _service.ExecutePutawayAsync(putawayId, "testuser")
        );

        Assert.Contains("Cannot execute putaway with status", exception.Message);
    }

    #endregion

    #region ClosePutawayAsync Tests

    [Fact]
    public async Task ClosePutawayAsync_WithCompletedStatus_UpdatesStatusToClosed()
    {
        // Arrange
        var putawayId = 1;
        var putaway = new Putaway { Id = putawayId, Status = PutawayStatus.Completed };

        _mockPutawayRepo.Setup(r => r.GetByIdAsync(putawayId)).ReturnsAsync(putaway);

        // Act
        await _service.ClosePutawayAsync(putawayId, "testuser");

        // Assert
        Assert.Equal(PutawayStatus.Closed, putaway.Status);
        Assert.NotNull(putaway.ClosedOn);
        Assert.Equal("testuser", putaway.ClosedBy);
        _mockPutawayRepo.Verify(r => r.Update(putaway), Times.Once);
        _mockUnitOfWork.Verify(uow => uow.CompleteAsync(), Times.Once);
    }

    [Fact]
    public async Task ClosePutawayAsync_WithNonCompletedStatus_ThrowsException()
    {
        // Arrange
        var putawayId = 1;
        var putaway = new Putaway { Id = putawayId, Status = PutawayStatus.Assigned };

        _mockPutawayRepo.Setup(r => r.GetByIdAsync(putawayId)).ReturnsAsync(putaway);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _service.ClosePutawayAsync(putawayId, "testuser")
        );

        Assert.Contains("Cannot close putaway with status", exception.Message);
    }

    #endregion

    #region GetPagedPutawaysAsync Tests

    [Fact]
    public async Task GetPagedPutawaysAsync_WithSearchTerm_ReturnsFilteredResults()
    {
        // Arrange
        var putaways = new List<Putaway>
        {
            new Putaway
            {
                Id = 1,
                Qty = 50,
                Status = PutawayStatus.Pending,
                ReceiptItem = new ReceiptItem { SKU = "ABC-123", Product = new Product { Name = "Product A" } }
            },
            new Putaway
            {
                Id = 2,
                Qty = 60,
                Status = PutawayStatus.Completed,
                ReceiptItem = new ReceiptItem { SKU = "XYZ-456", Product = new Product { Name = "Product B" } }
            }
        }.AsReadOnly();

        _mockPutawayRepo.Setup(r => r.GetPagedListAsync(
            It.IsAny<int>(), It.IsAny<int>(),
            It.IsAny<System.Linq.Expressions.Expression<Func<Putaway, bool>>>(),
            It.IsAny<Func<IQueryable<Putaway>, IOrderedQueryable<Putaway>>>(),
            It.IsAny<string>()))
            .ReturnsAsync((putaways, 2));

        // Act
        var (result, totalCount) = await _service.GetPagedPutawaysAsync(1, 10, "ABC", null);

        // Assert
        Assert.Equal(2, totalCount);
        Assert.Equal(2, result.Count());
        _mockPutawayRepo.Verify(r => r.GetPagedListAsync(
            1, 10,
            It.IsAny<System.Linq.Expressions.Expression<Func<Putaway, bool>>>(),
            It.IsAny<Func<IQueryable<Putaway>, IOrderedQueryable<Putaway>>>(),
            "ReceiptItem,ReceiptItem.Product"), Times.Once);
    }

    #endregion
}

/*
 * To run these tests:
 * 1. Ensure xUnit test project exists or create one:
 *    dotnet new xunit -n WMS.Tests
 * 
 * 2. Add project references:
 *    dotnet add WMS.Tests reference WMS.BLL/WMS.BLL.csproj
 *    dotnet add WMS.Tests reference WMS.DL/WMS.DL.csproj
 * 
 * 3. Add required NuGet packages:
 *    dotnet add package Moq
 *    dotnet add package xunit
 *    dotnet add package xunit.runner.visualstudio
 *    dotnet add package Microsoft.EntityFrameworkCore.InMemory
 * 
 * 4. Run tests:
 *    dotnet test
 *    
 * 5. Run with coverage:
 *    dotnet test /p:CollectCoverage=true
 */

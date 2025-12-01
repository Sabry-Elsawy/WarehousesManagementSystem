using Xunit;
using Moq;
using WMS.BLL.Services;
using WMS.DAL;
using WMS.DAL.UnitOfWork;
using WMS.DAL.Contract;

namespace WMS.Tests.Services;

public class ProductServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IGenericRepository<Product, int>> _mockRepo;
    private readonly ProductService _service;

    public ProductServiceTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockRepo = new Mock<IGenericRepository<Product, int>>();
        
        _mockUnitOfWork
            .Setup(uow => uow.GetRepository<Product, int>())
            .Returns(_mockRepo.Object);

        _service = new ProductService(_mockUnitOfWork.Object);
    }

    [Fact]
    public async Task CreateAsync_WithValidProduct_CallsRepositoryAndCompletes()
    {
        // Arrange
        var product = new Product
        {
            Code = "TEST001",
            Name = "Test Product",
            UnitOfMeasure = "PCS",
            IsActive = true
        };

        // Setup: Code is unique (no existing products with same code)
        _mockRepo
            .Setup(r => r.GetPagedListAsync(
                1, 1,
                It.IsAny<System.Linq.Expressions.Expression<Func<Product, bool>>>(),
                null,
                ""))
            .ReturnsAsync((new List<Product>().AsReadOnly(), 0));

        // Act
        await _service.CreateAsync(product);

        // Assert
        _mockRepo.Verify(r => r.AddAsync(product), Times.Once);
        _mockUnitOfWork.Verify(uow => uow.CompleteAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithDuplicateCode_ThrowsArgumentException()
    {
        // Arrange
        var product = new Product
        {
            Code = "DUPLICATE",
            Name = "Test Product"
        };

        // Setup: Code already exists
        var existingProduct = new Product { Id = 1, Code = "DUPLICATE" };
        _mockRepo
            .Setup(r => r.GetPagedListAsync(
                1, 1,
                It.IsAny<System.Linq.Expressions.Expression<Func<Product, bool>>>(),
                null,
                ""))
            .ReturnsAsync((new List<Product> { existingProduct }.AsReadOnly(), 1));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => _service.CreateAsync(product)
        );

        Assert.Contains("already exists", exception.Message);
        _mockRepo.Verify(r => r.AddAsync(It.IsAny<Product>()), Times.Never);
    }

    [Fact]
    public async Task GetPagedListAsync_WithSearchTerm_ReturnsFilteredResults()
    {
        // Arrange
        var products = new List<Product>
        {
            new Product { Id = 1, Code = "PRD001", Name = "Widget A" },
            new Product { Id = 2, Code = "PRD002", Name = "Widget B" }
        }.AsReadOnly();

        _mockRepo
            .Setup(r => r.GetPagedListAsync(
                1, 10,
                It.IsAny<System.Linq.Expressions.Expression<Func<Product, bool>>>(),
                It.IsAny<Func<IQueryable<Product>, IOrderedQueryable<Product>>>(),
                ""))
            .ReturnsAsync((products, 2));

        // Act
        var (result, totalCount) = await _service.GetPagedListAsync(
            pageNumber: 1,
            pageSize: 10,
            searchTerm: "Widget"
        );

        // Assert
        Assert.Equal(2, totalCount);
        Assert.Equal(2, result.Count);
        _mockRepo.Verify(
            r => r.GetPagedListAsync(1, 10, It.IsAny<System.Linq.Expressions.Expression<Func<Product, bool>>>(), It.IsAny<Func<IQueryable<Product>, IOrderedQueryable<Product>>>(), ""),
            Times.Once
        );
    }

    [Fact]
    public async Task ToggleActiveAsync_WithValidId_TogglesIsActiveFlag()
    {
        // Arrange
        var product = new Product
        {
            Id = 1,
            Code = "PRD001",
            Name = "Test Product",
            IsActive = true
        };

        _mockRepo
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(product);

        // Act
        await _service.ToggleActiveAsync(1);

        // Assert
        Assert.False(product.IsActive); // Should be toggled to false
        _mockRepo.Verify(r => r.Update(product), Times.Once);
        _mockUnitOfWork.Verify(uow => uow.CompleteAsync(), Times.Once);
    }

    [Fact]
    public async Task ToggleActiveAsync_WithInvalidId_ThrowsArgumentException()
    {
        // Arrange
        _mockRepo
            .Setup(r => r.GetByIdAsync(999))
            .ReturnsAsync((Product?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => _service.ToggleActiveAsync(999)
        );

        Assert.Contains("not found", exception.Message);
    }
}

/*
 * To run these tests:
 * 1. Create a new xUnit test project:
 *    dotnet new xunit -n WMS.Tests
 * 
 * 2. Add project references:
 *    dotnet add reference ../WMS.BLL/WMS.BLL.csproj
 *    dotnet add reference ../WMS.DL/WMS.DAL.csproj
 * 
 * 3. Add NuGet packages:
 *    dotnet add package Moq
 *    dotnet add package xunit
 *    dotnet add package xunit.runner.visualstudio
 * 
 * 4. Run tests:
 *    dotnet test
 */

using Microsoft.AspNetCore.Mvc;
using Moq;
using StoreManagement.API.Controllers;
using StoreManagement.Domain.DTOs;
using StoreManagement.Domain.Interfaces;
using StoreManagement.Domain.Interfaces.Services;
using Xunit;

namespace StoreManagement.Tests.Controllers;

public class StoresControllerTests
{
    private readonly Mock<IStoreService> _mockStoreService;
    private readonly StoresController _controller;

    public StoresControllerTests()
    {
        _mockStoreService = new Mock<IStoreService>();
        _controller = new StoresController(_mockStoreService.Object);
    }

    [Fact]
    public async Task GetStores_ReturnsOkResult_WithListOfStores()
    {
        var companyId = Guid.NewGuid();
        // Arrange
        var stores = new List<StoreDto>
        {
            new StoreDto { Id = Guid.NewGuid(), Name = "Store 1", CompanyId = companyId },
            new StoreDto { Id = Guid.NewGuid(), Name = "Store 2", CompanyId = companyId }
        };
        _mockStoreService.Setup(s => s.GetAllStoresAsync()).ReturnsAsync(stores);

        // Act
        var result = await _controller.GetStores();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedStores = Assert.IsAssignableFrom<IEnumerable<StoreDto>>(okResult.Value);
        Assert.Equal(2, returnedStores.Count());
    }

    [Fact]
    public async Task GetStore_WithValidId_ReturnsOkResult()
    {
        // Arrange
        var storeId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var store = new StoreDto { Id = storeId, Name = "Test Store", CompanyId = companyId };
        _mockStoreService.Setup(s => s.GetStoreByIdAsync(storeId)).ReturnsAsync(store);

        // Act
        var result = await _controller.GetStore(storeId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedStore = Assert.IsType<StoreDto>(okResult.Value);
        Assert.Equal(storeId, returnedStore.Id);
    }

    [Fact]
    public async Task GetStore_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var storeId = Guid.NewGuid();
        _mockStoreService.Setup(s => s.GetStoreByIdAsync(storeId)).ReturnsAsync((StoreDto?)null);

        // Act
        var result = await _controller.GetStore(storeId);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task CreateStore_WithValidData_ReturnsCreatedResult()
    {
        // Arrange
        var createStoreDto = new CreateStoreDto
        {
            Name = "New Store",
            CompanyId = Guid.NewGuid(),
            Address = "123 Main St"
        };
        var createdStore = new StoreDto
        {
            Id = Guid.NewGuid(),
            Name = createStoreDto.Name,
            CompanyId = createStoreDto.CompanyId,
            Address = createStoreDto.Address
        };
        _mockStoreService.Setup(s => s.CompanyExistsAsync(createStoreDto.CompanyId)).ReturnsAsync(true);
        _mockStoreService.Setup(s => s.CreateStoreAsync(createStoreDto)).ReturnsAsync(createdStore);

        // Act
        var result = await _controller.CreateStore(createStoreDto);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var returnedStore = Assert.IsType<StoreDto>(createdResult.Value);
        Assert.Equal(createdStore.Id, returnedStore.Id);
    }

    [Fact]
    public async Task UpdateStore_WithValidData_ReturnsOkResult()
    {
        // Arrange
        var storeId = Guid.NewGuid();
        var updateStoreDto = new UpdateStoreDto
        {
            Name = "Updated Store",
            CompanyId = Guid.NewGuid()
        };
        var updatedStore = new StoreDto
        {
            Id = storeId,
            Name = updateStoreDto.Name,
            CompanyId = updateStoreDto.CompanyId
        };
        _mockStoreService.Setup(s => s.StoreExistsAsync(storeId)).ReturnsAsync(true);
        _mockStoreService.Setup(s => s.CompanyExistsAsync(updateStoreDto.CompanyId)).ReturnsAsync(true);
        _mockStoreService.Setup(s => s.UpdateStoreAsync(storeId, updateStoreDto)).ReturnsAsync(updatedStore);

        // Act
        var result = await _controller.UpdateStore(storeId, updateStoreDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedStore = Assert.IsType<StoreDto>(okResult.Value);
        Assert.Equal(storeId, returnedStore.Id);
    }

    [Fact]
    public async Task DeleteStore_WithValidId_ReturnsNoContent()
    {
        // Arrange
        var storeId = Guid.NewGuid();
        _mockStoreService.Setup(s => s.StoreExistsAsync(storeId)).ReturnsAsync(true);

        // Act
        var result = await _controller.DeleteStore(storeId);

        // Assert
        Assert.IsType<NoContentResult>(result);
        _mockStoreService.Verify(s => s.DeleteStoreAsync(storeId), Times.Once);
    }
}

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
        var companyCode = 1001;
        // Arrange
        var stores = new List<StoreDto>
        {
            new StoreDto { Id = Guid.NewGuid(), Name = "Store 1", Code = 101, CompanyId = companyId, CompanyCode = companyCode, CompanyName = "Test Company" },
            new StoreDto { Id = Guid.NewGuid(), Name = "Store 2", Code = 102, CompanyId = companyId, CompanyCode = companyCode, CompanyName = "Test Company" }
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
        var companyCode = 1001;
        var store = new StoreDto { Id = storeId, Name = "Test Store", Code = 101, CompanyId = companyId, CompanyCode = companyCode, CompanyName = "Test Company" };
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
        var companyCode = 1001;
        var createStoreDto = new CreateStoreDto
        {
            Name = "New Store",
            Code = 101,
            CompanyCode = companyCode,
            Address = "123 Main St"
        };
        var createdStore = new StoreDto
        {
            Id = Guid.NewGuid(),
            Name = createStoreDto.Name,
            Code = createStoreDto.Code,
            CompanyCode = createStoreDto.CompanyCode,
            CompanyName = "Test Company",
            Address = createStoreDto.Address
        };
        _mockStoreService.Setup(s => s.CompanyExistsByCodeAsync(createStoreDto.CompanyCode)).ReturnsAsync(true);
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
        var companyCode = 1001;
        var updateStoreDto = new UpdateStoreDto
        {
            Name = "Updated Store",
            Code = 101,
            CompanyCode = companyCode
        };
        var updatedStore = new StoreDto
        {
            Id = storeId,
            Name = updateStoreDto.Name,
            Code = updateStoreDto.Code,
            CompanyCode = updateStoreDto.CompanyCode,
            CompanyName = "Test Company"
        };
        _mockStoreService.Setup(s => s.StoreExistsAsync(storeId)).ReturnsAsync(true);
        _mockStoreService.Setup(s => s.CompanyExistsByCodeAsync(updateStoreDto.CompanyCode)).ReturnsAsync(true);
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

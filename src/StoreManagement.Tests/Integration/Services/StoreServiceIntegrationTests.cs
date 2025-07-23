using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using StoreManagement.Domain.DTOs;
using StoreManagement.Domain.Entities;
using StoreManagement.Domain.Interfaces.Repositories;
using StoreManagement.Domain.Interfaces.Services;
using StoreManagement.Tests.Integration.Base;

namespace StoreManagement.Tests.Integration.Services;

[Collection("IntegrationTests")]
public class StoreServiceIntegrationTests : IntegrationTestBase
{
    private readonly IStoreService _storeService;

    public StoreServiceIntegrationTests()
    {
        _storeService = GetService<IStoreService>();
    }

    [Fact]
    public async Task Should_CreateStore_WithValidCompany_Successfully()
    {
        // Arrange
        var company = await CreateTestCompany(200);

        const string storeName = "Test Store";
        const int storeCode = 300;
        const string address = "123 Test Street";
        const bool isActive = true;

        var createDto = new CreateStoreDto
        {
            Name = storeName,
            Code = storeCode,
            Address = address,
            IsActive = isActive,
            CompanyCode = company.Code
        };

        // Act
        var result = await _storeService.CreateStoreAsync(createDto);

        // Assert - Verify service result
        result.Should().NotBeNull();
        result.Name.Should().Be(storeName);
        result.Code.Should().Be(storeCode);
        result.Address.Should().Be(address);
        result.IsActive.Should().Be(isActive);
        result.Id.Should().NotBeEmpty();

        // Assert - Verify persistence in database
        using var scope = ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetService<IStoreRepository>();
        var persistedStore = await repository!.GetByIdAsync(result.Id);

        persistedStore.Should().NotBeNull();
        persistedStore!.Name.Should().Be(storeName);
        persistedStore.Code.Should().Be(storeCode);
        persistedStore.Address.Should().Be(address);
        persistedStore.IsActive.Should().Be(isActive);
        persistedStore.CompanyId.Should().Be(company.Id);
    }

    [Fact]
    public async Task Should_GetStoreById_Successfully()
    {
        // Arrange
        var company = await CreateTestCompany(201);
        var store = new Store
        {
            Id = Guid.NewGuid(),
            Name = "Test Store",
            Code = 301,
            Address = "123 Test Street",
            IsActive = true,
            CompanyId = company.Id,
            CreatedAt = DateTime.UtcNow
        };

        await AddEntities(store);

        // Act
        var result = await _storeService.GetStoreByIdAsync(store.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(store.Id);
        result.Name.Should().Be(store.Name);
        result.Code.Should().Be(store.Code);
        result.Address.Should().Be(store.Address);
        result.IsActive.Should().Be(store.IsActive);
    }

    [Fact]
    public async Task Should_GetAllStores_Successfully()
    {
        // Arrange
        var company = await CreateTestCompany(202);
        var stores = new[]
        {
            new Store
            {
                Id = Guid.NewGuid(),
                Name = "Store 1",
                Code = 302,
                Address = "Address 1",
                IsActive = true,
                CompanyId = company.Id,
                CreatedAt = DateTime.UtcNow
            },
            new Store
            {
                Id = Guid.NewGuid(),
                Name = "Store 2",
                Code = 303,
                Address = "Address 2",
                IsActive = false,
                CompanyId = company.Id,
                CreatedAt = DateTime.UtcNow
            }
        };

        await AddEntities(stores);

        // Act
        var result = await _storeService.GetAllStoresAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().Contain(s => s.Name == "Store 1" && s.Code == 302);
        result.Should().Contain(s => s.Name == "Store 2" && s.Code == 303);
    }

    [Fact]
    public async Task Should_GetStoresByCompanyId_Successfully()
    {
        // Arrange
        var company1 = await CreateTestCompany(203);
        var company2 = await CreateTestCompany(204);

        var stores = new[]
        {
            new Store
            {
                Id = Guid.NewGuid(),
                Name = "Company 1 Store",
                Code = 304,
                Address = "Address 1",
                IsActive = true,
                CompanyId = company1.Id,
                CreatedAt = DateTime.UtcNow
            },
            new Store
            {
                Id = Guid.NewGuid(),
                Name = "Company 2 Store",
                Code = 305,
                Address = "Address 2",
                IsActive = true,
                CompanyId = company2.Id,
                CreatedAt = DateTime.UtcNow
            }
        };

        await AddEntities(stores);

        // Act
        var result = await _storeService.GetStoresByCompanyAsync(company1.Id);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result.First().Name.Should().Be("Company 1 Store");
        result.First().Code.Should().Be(304);
    }

    [Fact]
    public async Task Should_GetStoresByCompanyCode_Successfully()
    {
        // Arrange
        var company = await CreateTestCompany(205);
        var store = new Store
        {
            Id = Guid.NewGuid(),
            Name = "Store by Company Code",
            Code = 306,
            Address = "Address",
            IsActive = true,
            CompanyId = company.Id,
            CreatedAt = DateTime.UtcNow
        };

        await AddEntities(store);

        // Act
        var result = await _storeService.GetStoresByCompanyCodeAsync(company.Code);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result.First().Name.Should().Be("Store by Company Code");
        result.First().Code.Should().Be(306);
    }

    [Fact]
    public async Task Should_UpdateStore_Successfully()
    {
        // Arrange
        var company = await CreateTestCompany(206);
        var store = new Store
        {
            Id = Guid.NewGuid(),
            Name = "Original Store",
            Code = 307,
            Address = "Original Address",
            IsActive = true,
            CompanyId = company.Id,
            CreatedAt = DateTime.UtcNow
        };

        await AddEntities(store);

        var updateDto = new UpdateStoreDto
        {
            Name = "Updated Store",
            Code = 308,
            Address = "Updated Address",
            IsActive = false,
            CompanyCode = company.Code
        };

        // Act
        var result = await _storeService.UpdateStoreAsync(store.Id, updateDto);

        // Assert - Verify service result
        result.Should().NotBeNull();
        result.Id.Should().Be(store.Id);
        result.Name.Should().Be("Updated Store");
        result.Code.Should().Be(308);
        result.Address.Should().Be("Updated Address");
        result.IsActive.Should().BeFalse();

        // Assert - Verify persistence in database
        using var scope = ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetService<IStoreRepository>();
        var persistedStore = await repository!.GetByIdAsync(store.Id);

        persistedStore.Should().NotBeNull();
        persistedStore!.Name.Should().Be("Updated Store");
        persistedStore.Code.Should().Be(308);
        persistedStore.Address.Should().Be("Updated Address");
        persistedStore.IsActive.Should().BeFalse();
        persistedStore.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task Should_PatchStore_Successfully()
    {
        // Arrange
        var company = await CreateTestCompany(207);
        var store = new Store
        {
            Id = Guid.NewGuid(),
            Name = "Original Store",
            Code = 309,
            Address = "Original Address",
            IsActive = true,
            CompanyId = company.Id,
            CreatedAt = DateTime.UtcNow
        };

        await AddEntities(store);

        var patchDto = new PatchStoreDto
        {
            Name = "Patched Store"
            // Only updating name, leaving other fields unchanged
        };

        // Act
        var result = await _storeService.PatchStoreAsync(store.Id, patchDto);

        // Assert - Verify service result
        result.Should().NotBeNull();
        result.Id.Should().Be(store.Id);
        result.Name.Should().Be("Patched Store");
        result.Code.Should().Be(309); // Should remain unchanged
        result.Address.Should().Be("Original Address"); // Should remain unchanged
        result.IsActive.Should().BeTrue(); // Should remain unchanged

        // Assert - Verify persistence in database
        using var scope = ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetService<IStoreRepository>();
        var persistedStore = await repository!.GetByIdAsync(store.Id);

        persistedStore.Should().NotBeNull();
        persistedStore!.Name.Should().Be("Patched Store");
        persistedStore.Code.Should().Be(309);
        persistedStore.Address.Should().Be("Original Address");
        persistedStore.IsActive.Should().BeTrue();
        persistedStore.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task Should_DeleteStore_Successfully()
    {
        // Arrange
        var company = await CreateTestCompany(208);
        var store = new Store
        {
            Id = Guid.NewGuid(),
            Name = "Store to Delete",
            Code = 310,
            Address = "Address",
            IsActive = true,
            CompanyId = company.Id,
            CreatedAt = DateTime.UtcNow
        };

        await AddEntities(store);

        // Act
        await _storeService.DeleteStoreAsync(store.Id);

        // Assert - Verify store is deleted from database
        using var scope = ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetService<IStoreRepository>();
        var deletedStore = await repository!.GetByIdAsync(store.Id);

        deletedStore.Should().BeNull();
    }

    [Fact]
    public async Task Should_CheckStoreExists_ReturnTrue_WhenExists()
    {
        // Arrange
        var company = await CreateTestCompany(209);
        var store = new Store
        {
            Id = Guid.NewGuid(),
            Name = "Existing Store",
            Code = 311,
            Address = "Address",
            IsActive = true,
            CompanyId = company.Id,
            CreatedAt = DateTime.UtcNow
        };

        await AddEntities(store);

        // Act
        var exists = await _storeService.StoreExistsAsync(store.Id);

        // Assert
        exists.Should().BeTrue();
    }

    [Fact]
    public async Task Should_CheckStoreExists_ReturnFalse_WhenNotExists()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var exists = await _storeService.StoreExistsAsync(nonExistentId);

        // Assert
        exists.Should().BeFalse();
    }

    [Fact]
    public async Task Should_ThrowException_WhenCreatingStoreWithNonExistentCompany()
    {
        // Arrange
        var createDto = new CreateStoreDto
        {
            Name = "Test Store",
            Code = 312,
            Address = "Address",
            IsActive = true,
            CompanyCode = 999 // Non-existent company code
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => _storeService.CreateStoreAsync(createDto));

        exception.Message.Should().Contain("Company with code 999 not found");
    }

    [Fact]
    public async Task Should_ThrowException_WhenUpdatingNonExistentStore()
    {
        // Arrange
        var company = await CreateTestCompany(210);
        var nonExistentId = Guid.NewGuid();
        var updateDto = new UpdateStoreDto
        {
            Name = "Updated Store",
            Code = 313,
            Address = "Address",
            IsActive = true,
            CompanyCode = company.Code
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => _storeService.UpdateStoreAsync(nonExistentId, updateDto));

        exception.Message.Should().Contain($"Store with ID {nonExistentId} not found");
    }

    [Fact]
    public async Task Should_ThrowException_WhenPatchingNonExistentStore()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        var patchDto = new PatchStoreDto
        {
            Name = "Patched Store"
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => _storeService.PatchStoreAsync(nonExistentId, patchDto));

        exception.Message.Should().Contain($"Store with ID {nonExistentId} not found");
    }

    private async Task<Company> CreateTestCompany(int code)
    {
        var company = new Company
        {
            Id = Guid.NewGuid(),
            Name = $"Test Company {code}",
            Code = code,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await AddEntities(company);
        return company;
    }
}

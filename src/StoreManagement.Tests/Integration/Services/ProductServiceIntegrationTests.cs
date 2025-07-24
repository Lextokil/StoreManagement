using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using StoreManagement.Domain.DTOs;
using StoreManagement.Domain.Entities;
using StoreManagement.Domain.Interfaces.Repositories;
using StoreManagement.Domain.Interfaces.Services;
using StoreManagement.Tests.Integration.Base;
using System.Linq;

namespace StoreManagement.Tests.Integration.Services;

[Collection("IntegrationTests")]
public class ProductServiceIntegrationTests : IntegrationTestBase
{
    private readonly IProductService _productService;

    public ProductServiceIntegrationTests()
    {
        _productService = GetService<IProductService>();
    }

    [Fact]
    public async Task Should_CreateProduct_WithValidStore_Successfully()
    {
        // Arrange
        var (company, store) = await CreateTestCompanyAndStore(400, 500);

        const string productName = "Test Product";
        const int productCode = 600;
        const string description = "Test product description";
        const decimal price = 99.99m;
        const bool isActive = true;

        var createDto = new CreateProductDto
        {
            Name = productName,
            Code = productCode,
            Description = description,
            Price = price,
            IsActive = isActive,
            StoreCode = store.Code
        };

        // Act
        var result = await _productService.CreateProductAsync(createDto);

        // Assert - Verify service result
        result.Should().NotBeNull();
        result.Name.Should().Be(productName);
        result.Code.Should().Be(productCode);
        result.Description.Should().Be(description);
        result.Price.Should().Be(price);
        result.IsActive.Should().Be(isActive);
        result.Id.Should().NotBeEmpty();

        // Assert - Verify persistence in database
        using var scope = ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetService<IProductRepository>();
        var persistedProduct = await repository!.GetByIdAsync(result.Id);

        persistedProduct.Should().NotBeNull();
        persistedProduct!.Name.Should().Be(productName);
        persistedProduct.Code.Should().Be(productCode);
        persistedProduct.Description.Should().Be(description);
        persistedProduct.Price.Should().Be(price);
        persistedProduct.IsActive.Should().Be(isActive);
        persistedProduct.StoreId.Should().Be(store.Id);
    }

    [Fact]
    public async Task Should_GetProductById_Successfully()
    {
        // Arrange
        var (company, store) = await CreateTestCompanyAndStore(401, 501);
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = "Test Product",
            Code = 601,
            Description = "Test description",
            Price = 49.99m,
            IsActive = true,
            StoreId = store.Id,
            CreatedAt = DateTime.UtcNow
        };

        await AddEntities(product);

        // Act
        var result = await _productService.GetProductByIdAsync(product.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(product.Id);
        result.Name.Should().Be(product.Name);
        result.Code.Should().Be(product.Code);
        result.Description.Should().Be(product.Description);
        result.Price.Should().Be(product.Price);
        result.IsActive.Should().Be(product.IsActive);
    }

    [Fact]
    public async Task Should_GetProductsByStoreCode_Successfully()
    {
        // Arrange
        var (company, store) = await CreateTestCompanyAndStore(402, 502);
        var products = new[]
        {
            new Product
            {
                Id = Guid.NewGuid(),
                Name = "Product 1",
                Code = 602,
                Description = "Description 1",
                Price = 10.00m,
                IsActive = true,
                StoreId = store.Id,
                CreatedAt = DateTime.UtcNow
            },
            new Product
            {
                Id = Guid.NewGuid(),
                Name = "Product 2",
                Code = 603,
                Description = "Description 2",
                Price = 20.00m,
                IsActive = false, 
                StoreId = store.Id,
                CreatedAt = DateTime.UtcNow
            },
            new Product
            {
                Id = Guid.NewGuid(),
                Name = "Product 3",
                Code = 604,
                Description = null, 
                Price = 30.00m,
                IsActive = true,
                StoreId = store.Id,
                CreatedAt = DateTime.UtcNow
            }
        };

        await AddEntities(products);

        var result = await _productService.GetProductsByStoreCodeAsync(store.Code);

        result.Should().NotBeNull();
    }

    [Fact]
    public async Task Should_UpdateProduct_Successfully()
    {
        // Arrange
        var (company, store) = await CreateTestCompanyAndStore(403, 503);
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = "Original Product",
            Code = 604,
            Description = "Original description",
            Price = 30.00m,
            IsActive = true,
            StoreId = store.Id,
            CreatedAt = DateTime.UtcNow
        };

        await AddEntities(product);

        var updateDto = new UpdateProductDto
        {
            Name = "Updated Product",
            Code = 605,
            Description = "Updated description",
            Price = 35.00m,
            IsActive = false,
            StoreCode = store.Code
        };

        // Act
        var result = await _productService.UpdateProductAsync(product.Id, updateDto);

        // Assert - Verify service result
        result.Should().NotBeNull();
        result.Id.Should().Be(product.Id);
        result.Name.Should().Be("Updated Product");
        result.Code.Should().Be(605);
        result.Description.Should().Be("Updated description");
        result.Price.Should().Be(35.00m);
        result.IsActive.Should().BeFalse();

        // Assert - Verify persistence in database
        using var scope = ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetService<IProductRepository>();
        var persistedProduct = await repository!.GetByIdAsync(product.Id);

        persistedProduct.Should().NotBeNull();
        persistedProduct!.Name.Should().Be("Updated Product");
        persistedProduct.Code.Should().Be(605);
        persistedProduct.Description.Should().Be("Updated description");
        persistedProduct.Price.Should().Be(35.00m);
        persistedProduct.IsActive.Should().BeFalse();
        persistedProduct.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task Should_PatchProduct_Successfully()
    {
        // Arrange
        var (company, store) = await CreateTestCompanyAndStore(404, 504);
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = "Original Product",
            Code = 606,
            Description = "Original description",
            Price = 40.00m,
            IsActive = true,
            StoreId = store.Id,
            CreatedAt = DateTime.UtcNow
        };

        await AddEntities(product);

        var patchDto = new PatchProductDto
        {
            Name = "Patched Product",
            Price = 45.00m
            // Only updating name and price, leaving other fields unchanged
        };

        // Act
        var result = await _productService.PatchProductAsync(product.Id, patchDto);

        // Assert - Verify service result
        result.Should().NotBeNull();
        result.Id.Should().Be(product.Id);
        result.Name.Should().Be("Patched Product");
        result.Code.Should().Be(606); // Should remain unchanged
        result.Description.Should().Be("Original description"); // Should remain unchanged
        result.Price.Should().Be(45.00m);
        result.IsActive.Should().BeTrue(); // Should remain unchanged

        // Assert - Verify persistence in database
        using var scope = ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetService<IProductRepository>();
        var persistedProduct = await repository!.GetByIdAsync(product.Id);

        persistedProduct.Should().NotBeNull();
        persistedProduct!.Name.Should().Be("Patched Product");
        persistedProduct.Code.Should().Be(606);
        persistedProduct.Description.Should().Be("Original description");
        persistedProduct.Price.Should().Be(45.00m);
        persistedProduct.IsActive.Should().BeTrue();
        persistedProduct.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task Should_DeleteProduct_Successfully()
    {
        // Arrange
        var (company, store) = await CreateTestCompanyAndStore(405, 505);
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = "Product to Delete",
            Code = 607,
            Description = "Description",
            Price = 50.00m,
            IsActive = true,
            StoreId = store.Id,
            CreatedAt = DateTime.UtcNow
        };

        await AddEntities(product);

        // Act
        await _productService.DeleteProductAsync(product.Id);

        // Assert - Verify product is deleted from database
        using var scope = ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetService<IProductRepository>();
        var deletedProduct = await repository!.GetByIdAsync(product.Id);

        deletedProduct.Should().BeNull();
    }

    [Fact]
    public async Task Should_CheckProductExists_ReturnTrue_WhenExists()
    {
        // Arrange
        var (company, store) = await CreateTestCompanyAndStore(406, 506);
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = "Existing Product",
            Code = 608,
            Description = "Description",
            Price = 60.00m,
            IsActive = true,
            StoreId = store.Id,
            CreatedAt = DateTime.UtcNow
        };

        await AddEntities(product);

        // Act
        var exists = await _productService.ProductExistsAsync(product.Id);

        // Assert
        exists.Should().BeTrue();
    }

    [Fact]
    public async Task Should_CheckProductExists_ReturnFalse_WhenNotExists()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var exists = await _productService.ProductExistsAsync(nonExistentId);

        // Assert
        exists.Should().BeFalse();
    }

    [Fact]
    public async Task Should_CheckStoreExistsByCode_ReturnTrue_WhenExists()
    {
        // Arrange
        var (company, store) = await CreateTestCompanyAndStore(407, 507);

        // Act
        var exists = await _productService.StoreExistsByCodeAsync(store.Code);

        // Assert
        exists.Should().BeTrue();
    }

    [Fact]
    public async Task Should_CheckStoreExistsByCode_ReturnFalse_WhenNotExists()
    {
        // Arrange
        const int nonExistentStoreCode = 999;

        // Act
        var exists = await _productService.StoreExistsByCodeAsync(nonExistentStoreCode);

        // Assert
        exists.Should().BeFalse();
    }

    [Fact]
    public async Task Should_ThrowException_WhenCreatingProductWithNonExistentStore()
    {
        // Arrange
        var createDto = new CreateProductDto
        {
            Name = "Test Product",
            Code = 609,
            Description = "Description",
            Price = 70.00m,
            IsActive = true,
            StoreCode = 999 // Non-existent store code
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => _productService.CreateProductAsync(createDto));

        exception.Message.Should().Contain("Store with code 999 not found");
    }

    [Fact]
    public async Task Should_ThrowException_WhenUpdatingNonExistentProduct()
    {
        // Arrange
        var (company, store) = await CreateTestCompanyAndStore(408, 508);
        var nonExistentId = Guid.NewGuid();
        var updateDto = new UpdateProductDto
        {
            Name = "Updated Product",
            Code = 610,
            Description = "Description",
            Price = 80.00m,
            IsActive = true,
            StoreCode = store.Code
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => _productService.UpdateProductAsync(nonExistentId, updateDto));

        exception.Message.Should().Contain($"Product with ID {nonExistentId} not found");
    }

    [Fact]
    public async Task Should_ThrowException_WhenPatchingNonExistentProduct()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        var patchDto = new PatchProductDto
        {
            Name = "Patched Product"
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => _productService.PatchProductAsync(nonExistentId, patchDto));

        exception.Message.Should().Contain($"Product with ID {nonExistentId} not found");
    }

    [Fact]
    public async Task Should_ThrowException_WhenUpdatingProductWithNonExistentStore()
    {
        // Arrange
        var (company, store) = await CreateTestCompanyAndStore(409, 509);
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = "Test Product",
            Code = 611,
            Description = "Description",
            Price = 90.00m,
            IsActive = true,
            StoreId = store.Id,
            CreatedAt = DateTime.UtcNow
        };

        await AddEntities(product);

        var updateDto = new UpdateProductDto
        {
            Name = "Updated Product",
            Code = 612,
            Description = "Updated description",
            Price = 95.00m,
            IsActive = false,
            StoreCode = 999 // Non-existent store code
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => _productService.UpdateProductAsync(product.Id, updateDto));

        exception.Message.Should().Contain("Store with code 999 not found");
    }

    [Fact]
    public async Task Should_ThrowException_WhenGetProductsByNonExistentStoreCode()
    {
        // Arrange
        const int nonExistentStoreCode = 999;

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => _productService.GetProductsByStoreCodeAsync(nonExistentStoreCode));

        exception.Message.Should().Contain($"Store with code {nonExistentStoreCode} not found");
    }

    private async Task<(Company company, Store store)> CreateTestCompanyAndStore(int companyCode, int storeCode)
    {
        var company = new Company
        {
            Id = Guid.NewGuid(),
            Name = $"Test Company {companyCode}",
            Code = companyCode,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var store = new Store
        {
            Id = Guid.NewGuid(),
            Name = $"Test Store {storeCode}",
            Code = storeCode,
            Address = $"Address {storeCode}",
            IsActive = true,
            CompanyId = company.Id,
            CreatedAt = DateTime.UtcNow
        };

        await AddEntities(company, store);
        return (company, store);
    }
}

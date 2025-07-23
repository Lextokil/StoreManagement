using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using StoreManagement.Domain.DTOs;
using StoreManagement.Domain.Entities;
using StoreManagement.Domain.Interfaces.Repositories;
using StoreManagement.Domain.Interfaces.Services;
using StoreManagement.Tests.Integration.Base;

namespace StoreManagement.Tests.Integration.Services;

[Collection("IntegrationTests")]
public class CompanyServiceIntegrationTests : IntegrationTestBase
{
    private readonly ICompanyService _companyService;

    public CompanyServiceIntegrationTests()
    {
        _companyService = GetService<ICompanyService>();
    }

    [Fact]
    public async Task Should_CreateCompany_Successfully()
    {
        // Arrange
        const string name = "Test Company";
        const int code = 100;
        const bool isActive = true;

        var createDto = new CreateCompanyDto
        {
            Name = name,
            Code = code,
            IsActive = isActive
        };

        // Act
        var result = await _companyService.CreateCompanyAsync(createDto);

        // Assert - Verify service result
        result.Should().NotBeNull();
        result.Name.Should().Be(name);
        result.Code.Should().Be(code);
        result.IsActive.Should().Be(isActive);
        result.Id.Should().NotBeEmpty();
        result.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));

        // Assert - Verify persistence in database
        using var scope = ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetService<ICompanyRepository>();
        var persistedCompany = await repository!.GetByIdAsync(result.Id);

        persistedCompany.Should().NotBeNull();
        persistedCompany!.Name.Should().Be(name);
        persistedCompany.Code.Should().Be(code);
        persistedCompany.IsActive.Should().Be(isActive);
    }

    [Fact]
    public async Task Should_GetCompanyById_Successfully()
    {
        // Arrange
        var company = new Company
        {
            Id = Guid.NewGuid(),
            Name = "Test Company",
            Code = 101,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await AddEntities(company);

        // Act
        var result = await _companyService.GetCompanyByIdAsync(company.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(company.Id);
        result.Name.Should().Be(company.Name);
        result.Code.Should().Be(company.Code);
        result.IsActive.Should().Be(company.IsActive);
    }

    [Fact]
    public async Task Should_GetCompanyByCode_Successfully()
    {
        // Arrange
        var company = new Company
        {
            Id = Guid.NewGuid(),
            Name = "Test Company",
            Code = 102,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await AddEntities(company);

        // Act
        var result = await _companyService.GetCompanyByCodeAsync(company.Code);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(company.Id);
        result.Name.Should().Be(company.Name);
        result.Code.Should().Be(company.Code);
        result.IsActive.Should().Be(company.IsActive);
    }

    [Fact]
    public async Task Should_GetAllCompanies_Successfully()
    {
        // Arrange
        var companies = new[]
        {
            new Company
            {
                Id = Guid.NewGuid(),
                Name = "Company 1",
                Code = 103,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new Company
            {
                Id = Guid.NewGuid(),
                Name = "Company 2",
                Code = 104,
                IsActive = false,
                CreatedAt = DateTime.UtcNow
            }
        };

        await AddEntities(companies);

        // Act
        var result = await _companyService.GetAllCompaniesAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().Contain(c => c.Name == "Company 1" && c.Code == 103);
        result.Should().Contain(c => c.Name == "Company 2" && c.Code == 104);
    }

    [Fact]
    public async Task Should_GetActiveCompanies_Successfully()
    {
        // Arrange
        var companies = new[]
        {
            new Company
            {
                Id = Guid.NewGuid(),
                Name = "Active Company",
                Code = 105,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new Company
            {
                Id = Guid.NewGuid(),
                Name = "Inactive Company",
                Code = 106,
                IsActive = false,
                CreatedAt = DateTime.UtcNow
            }
        };

        await AddEntities(companies);

        // Act
        var result = await _companyService.GetActiveCompaniesAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result.First().Name.Should().Be("Active Company");
        result.First().IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task Should_UpdateCompany_Successfully()
    {
        // Arrange
        var company = new Company
        {
            Id = Guid.NewGuid(),
            Name = "Original Company",
            Code = 107,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await AddEntities(company);

        var updateDto = new UpdateCompanyDto
        {
            Name = "Updated Company",
            Code = 108,
            IsActive = false
        };

        // Act
        var result = await _companyService.UpdateCompanyAsync(company.Id, updateDto);

        // Assert - Verify service result
        result.Should().NotBeNull();
        result.Id.Should().Be(company.Id);
        result.Name.Should().Be("Updated Company");
        result.Code.Should().Be(108);
        result.IsActive.Should().BeFalse();

        // Assert - Verify persistence in database
        using var scope = ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetService<ICompanyRepository>();
        var persistedCompany = await repository!.GetByIdAsync(company.Id);

        persistedCompany.Should().NotBeNull();
        persistedCompany!.Name.Should().Be("Updated Company");
        persistedCompany.Code.Should().Be(108);
        persistedCompany.IsActive.Should().BeFalse();
        persistedCompany.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task Should_PatchCompany_Successfully()
    {
        // Arrange
        var company = new Company
        {
            Id = Guid.NewGuid(),
            Name = "Original Company",
            Code = 109,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await AddEntities(company);

        var patchDto = new PatchCompanyDto
        {
            Name = "Patched Company"
            // Only updating name, leaving code and isActive unchanged
        };

        // Act
        var result = await _companyService.PatchCompanyAsync(company.Id, patchDto);

        // Assert - Verify service result
        result.Should().NotBeNull();
        result.Id.Should().Be(company.Id);
        result.Name.Should().Be("Patched Company");
        result.Code.Should().Be(109); // Should remain unchanged
        result.IsActive.Should().BeTrue(); // Should remain unchanged

        // Assert - Verify persistence in database
        using var scope = ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetService<ICompanyRepository>();
        var persistedCompany = await repository!.GetByIdAsync(company.Id);

        persistedCompany.Should().NotBeNull();
        persistedCompany!.Name.Should().Be("Patched Company");
        persistedCompany.Code.Should().Be(109);
        persistedCompany.IsActive.Should().BeTrue();
        persistedCompany.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task Should_DeleteCompany_Successfully()
    {
        // Arrange
        var company = new Company
        {
            Id = Guid.NewGuid(),
            Name = "Company to Delete",
            Code = 110,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await AddEntities(company);

        // Act
        await _companyService.DeleteCompanyAsync(company.Id);

        // Assert - Verify company is deleted from database
        using var scope = ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetService<ICompanyRepository>();
        var deletedCompany = await repository!.GetByIdAsync(company.Id);

        deletedCompany.Should().BeNull();
    }

    [Fact]
    public async Task Should_UpdateCompanyByCode_Successfully()
    {
        // Arrange
        var company = new Company
        {
            Id = Guid.NewGuid(),
            Name = "Original Company",
            Code = 111,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await AddEntities(company);

        var updateDto = new UpdateCompanyDto
        {
            Name = "Updated by Code",
            Code = 112,
            IsActive = false
        };

        // Act
        var result = await _companyService.UpdateCompanyByCodeAsync(111, updateDto);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Updated by Code");
        result.Code.Should().Be(112);
        result.IsActive.Should().BeFalse();

        // Verify in database
        using var scope = ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetService<ICompanyRepository>();
        var persistedCompany = await repository!.GetByCodeAsync(112);

        persistedCompany.Should().NotBeNull();
        persistedCompany!.Name.Should().Be("Updated by Code");
    }

    [Fact]
    public async Task Should_CheckCompanyExists_ReturnTrue_WhenExists()
    {
        // Arrange
        var company = new Company
        {
            Id = Guid.NewGuid(),
            Name = "Existing Company",
            Code = 113,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await AddEntities(company);

        // Act
        var exists = await _companyService.CompanyExistsAsync(company.Id);

        // Assert
        exists.Should().BeTrue();
    }

    [Fact]
    public async Task Should_CheckCompanyExists_ReturnFalse_WhenNotExists()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var exists = await _companyService.CompanyExistsAsync(nonExistentId);

        // Assert
        exists.Should().BeFalse();
    }

    [Fact]
    public async Task Should_ThrowException_WhenUpdatingNonExistentCompany()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        var updateDto = new UpdateCompanyDto
        {
            Name = "Updated Company",
            Code = 114,
            IsActive = true
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => _companyService.UpdateCompanyAsync(nonExistentId, updateDto));

        exception.Message.Should().Contain($"Company with ID {nonExistentId} not found");
    }

    [Fact]
    public async Task Should_ThrowException_WhenPatchingNonExistentCompany()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        var patchDto = new PatchCompanyDto
        {
            Name = "Patched Company"
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => _companyService.PatchCompanyAsync(nonExistentId, patchDto));

        exception.Message.Should().Contain($"Company with ID {nonExistentId} not found");
    }
}

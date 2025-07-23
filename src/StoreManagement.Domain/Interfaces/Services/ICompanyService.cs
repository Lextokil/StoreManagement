using StoreManagement.Domain.DTOs;

namespace StoreManagement.Domain.Interfaces.Services;

public interface ICompanyService
{
    Task<IEnumerable<CompanyDto>> GetAllCompaniesAsync();
    Task<IEnumerable<CompanyDto>> GetActiveCompaniesAsync();
    Task<CompanyDto?> GetCompanyByIdAsync(Guid id);
    Task<CompanyDto?> GetCompanyByCodeAsync(int code);
    Task<CompanyDto?> GetCompanyWithStoresAsync(Guid id);
    Task<CompanyDto?> GetCompanyWithStoresByCodeAsync(int code);
    Task<CompanyDto> CreateCompanyAsync(CreateCompanyDto createCompanyDto);
    Task<CompanyDto> UpdateCompanyAsync(Guid id, UpdateCompanyDto updateCompanyDto);
    Task<CompanyDto> UpdateCompanyByCodeAsync(int code, UpdateCompanyDto updateCompanyDto);
    Task<CompanyDto> PatchCompanyAsync(Guid id, PatchCompanyDto patchCompanyDto);
    Task<CompanyDto> PatchCompanyByCodeAsync(int code, PatchCompanyDto patchCompanyDto);
    Task DeleteCompanyAsync(Guid id);
    Task DeleteCompanyByCodeAsync(int code);
    Task<bool> CompanyExistsAsync(Guid id);
    Task<bool> CompanyExistsByCodeAsync(int code);
}

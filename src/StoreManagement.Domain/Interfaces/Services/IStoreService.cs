using StoreManagement.Domain.DTOs;

namespace StoreManagement.Domain.Interfaces.Services;

public interface IStoreService
{
    Task<IEnumerable<StoreDto>> GetAllStoresAsync();
    Task<IEnumerable<StoreDto>> GetStoresByCompanyAsync(Guid companyId);
    Task<IEnumerable<StoreDto>> GetStoresByCompanyCodeAsync(int companyCode);
    Task<StoreDto?> GetStoreByIdAsync(Guid id);
    Task<StoreDto> CreateStoreAsync(CreateStoreDto createStoreDto);
    Task<StoreDto> UpdateStoreAsync(Guid id, UpdateStoreDto updateStoreDto);
    Task<StoreDto> PatchStoreAsync(Guid id, PatchStoreDto patchStoreDto);
    Task DeleteStoreAsync(Guid id);
    Task<bool> StoreExistsAsync(Guid id);
    Task<bool> CompanyExistsAsync(Guid companyId);
    Task<bool> CompanyExistsByCodeAsync(int companyCode);
}

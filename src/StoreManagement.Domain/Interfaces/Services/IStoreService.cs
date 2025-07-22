using StoreManagement.Domain.DTOs;

namespace StoreManagement.Domain.Interfaces.Services;

public interface IStoreService
{
    Task<IEnumerable<StoreDto>> GetAllStoresAsync();
    Task<IEnumerable<StoreDto>> GetStoresByCompanyAsync(Guid companyId);
    Task<StoreDto?> GetStoreByIdAsync(Guid id);
    Task<StoreDto> CreateStoreAsync(CreateStoreDto createStoreDto);
    Task<StoreDto> UpdateStoreAsync(Guid id, UpdateStoreDto updateStoreDto);
    Task DeleteStoreAsync(Guid id);
    Task<bool> StoreExistsAsync(Guid id);
    Task<bool> CompanyExistsAsync(Guid companyId);
}

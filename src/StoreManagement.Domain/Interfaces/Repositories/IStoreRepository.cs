using StoreManagement.Domain.Entities;

namespace StoreManagement.Domain.Interfaces.Repositories;

public interface IStoreRepository : IGenericRepository<Store>
{
    Task<IEnumerable<Store>> GetByCompanyIdAsync(Guid companyId);
    Task<Store?> GetByCodeAsync(int code);
    Task<Store?> GetByCodesAsync(int companyCode, int storeCode);
    Task<Store?> GetStoreWithProductsAsync(Guid id);
    Task<IEnumerable<Store>> GetActiveStoresByCompanyAsync(Guid companyId);
}

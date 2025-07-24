using StoreManagement.Domain.Entities;

namespace StoreManagement.Domain.Interfaces.Repositories;

public interface IProductRepository : IGenericRepository<Product>
{
    Task<IEnumerable<Product>> GetByStoreIdAsync(Guid storeId);
    Task<IEnumerable<Product>> GetByCompanyIdAsync(Guid companyId);
    Task<Product?> GetByCodesAsync(int companyCode, int storeCode, int productCode);
    Task<string> GetProductsAsJsonAsync(Guid companyId);
    Task<string> GetProductsAsJsonByStoreIdAsync(Guid storeId);
}

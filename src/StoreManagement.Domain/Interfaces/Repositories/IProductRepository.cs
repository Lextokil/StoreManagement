using StoreManagement.Domain.Entities;

namespace StoreManagement.Domain.Interfaces.Repositories;

public interface IProductRepository : IGenericRepository<Product>
{
    Task<IEnumerable<Product>> GetByStoreIdAsync(Guid storeId);
    Task<IEnumerable<Product>> GetByCompanyIdAsync(Guid companyId);
    Task<IEnumerable<Product>> GetLowStockProductsAsync(Guid companyId, int threshold = 10);
    Task<string> GetProductsAsJsonAsync(Guid companyId);
}

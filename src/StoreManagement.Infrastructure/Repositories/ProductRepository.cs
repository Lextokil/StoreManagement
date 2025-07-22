using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using StoreManagement.Domain.Entities;
using StoreManagement.Domain.Interfaces;
using StoreManagement.Domain.Interfaces.Repositories;
using StoreManagement.Infrastructure.Data;

namespace StoreManagement.Infrastructure.Repositories;

public class ProductRepository : GenericRepository<Product>, IProductRepository
{
    public ProductRepository(StoreManagementDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Product>> GetByStoreIdAsync(Guid storeId)
    {
        return await _dbSet
            .Where(p => p.StoreId == storeId)
            .Include(p => p.Store)
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetByCompanyIdAsync(Guid companyId)
    {
        return await _dbSet
            .Include(p => p.Store)
            .Where(p => p.Store.CompanyId == companyId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetLowStockProductsAsync(Guid companyId, int threshold = 10)
    {
        return await _dbSet
            .Include(p => p.Store)
            .Where(p => p.Store.CompanyId == companyId && p.StockQuantity <= threshold)
            .ToListAsync();
    }

    public async Task<string> GetProductsAsJsonAsync(Guid companyId)
    {
        var companyIdParam = new SqlParameter("@CompanyId", companyId);
        
        var result = await _context.Database
            .SqlQueryRaw<string>("SELECT dbo.GetProductsAsJson(@CompanyId) AS Value", companyIdParam)
            .FirstOrDefaultAsync();

        return result ?? "[]";
    }
}

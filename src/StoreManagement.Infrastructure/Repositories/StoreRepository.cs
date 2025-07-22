using Microsoft.EntityFrameworkCore;
using StoreManagement.Domain.Entities;
using StoreManagement.Domain.Interfaces;
using StoreManagement.Domain.Interfaces.Repositories;
using StoreManagement.Infrastructure.Data;

namespace StoreManagement.Infrastructure.Repositories;

public class StoreRepository : GenericRepository<Store>, IStoreRepository
{
    public StoreRepository(StoreManagementDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Store>> GetByCompanyIdAsync(Guid companyId)
    {
        return await _dbSet.Where(s => s.CompanyId == companyId).ToListAsync();
    }

    public async Task<Store?> GetStoreWithProductsAsync(Guid id)
    {
        return await _dbSet
            .Include(s => s.Products)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<IEnumerable<Store>> GetActiveStoresByCompanyAsync(Guid companyId)
    {
        return await _dbSet
            .Where(s => s.CompanyId == companyId && s.IsActive)
            .ToListAsync();
    }
}

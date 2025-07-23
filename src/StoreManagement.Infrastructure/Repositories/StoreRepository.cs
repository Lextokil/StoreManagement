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

    public override async Task<IEnumerable<Store>> GetAllAsync()
    {
        return await _dbSet
            .Include(s => s.Company)
            .ToListAsync();
    }

    public override async Task<Store?> GetByIdAsync(Guid id)
    {
        return await _dbSet
            .Include(s => s.Company)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<IEnumerable<Store>> GetByCompanyIdAsync(Guid companyId)
    {
        return await _dbSet
            .Include(s => s.Company)
            .Where(s => s.CompanyId == companyId)
            .ToListAsync();
    }

    public async Task<Store?> GetByCodeAsync(int code)
    {
        return await _dbSet
            .Include(s => s.Company)
            .FirstOrDefaultAsync(s => s.Code == code);
    }

    public async Task<Store?> GetByCodesAsync(int companyCode, int storeCode)
    {
        return await _dbSet
            .Include(s => s.Company)
            .FirstOrDefaultAsync(s => s.Code == storeCode && s.Company.Code == companyCode);
    }

    public async Task<Store?> GetStoreWithProductsAsync(Guid id)
    {
        return await _dbSet
            .Include(s => s.Products)
            .Include(s => s.Company)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<IEnumerable<Store>> GetActiveStoresByCompanyAsync(Guid companyId)
    {
        return await _dbSet
            .Include(s => s.Company)
            .Where(s => s.CompanyId == companyId && s.IsActive)
            .ToListAsync();
    }
}

using Microsoft.EntityFrameworkCore;
using StoreManagement.Domain.Entities;
using StoreManagement.Domain.Interfaces;
using StoreManagement.Domain.Interfaces.Repositories;
using StoreManagement.Infrastructure.Data;

namespace StoreManagement.Infrastructure.Repositories;

public class CompanyRepository : GenericRepository<Company>, ICompanyRepository
{
    public CompanyRepository(StoreManagementDbContext context) : base(context)
    {
    }

    public async Task<Company?> GetByNameAsync(string name)
    {
        return await _dbSet.FirstOrDefaultAsync(c => c.Name == name);
    }

    public async Task<IEnumerable<Company>> GetActiveCompaniesAsync()
    {
        return await _dbSet.Where(c => c.IsActive).ToListAsync();
    }

    public async Task<Company?> GetCompanyWithStoresAsync(Guid id)
    {
        return await _dbSet
            .Include(c => c.Stores)
            .FirstOrDefaultAsync(c => c.Id == id);
    }
}

using StoreManagement.Domain.Entities;

namespace StoreManagement.Domain.Interfaces.Repositories;

public interface ICompanyRepository : IGenericRepository<Company>
{
    Task<Company?> GetByNameAsync(string name);
    Task<Company?> GetByCodeAsync(int code);
    Task<IEnumerable<Company>> GetActiveCompaniesAsync();
    Task<Company?> GetCompanyWithStoresAsync(Guid id);
    Task<Company?> GetCompanyWithStoresByCodeAsync(int code);
}

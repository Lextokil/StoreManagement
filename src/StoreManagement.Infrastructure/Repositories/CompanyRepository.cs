using Microsoft.Data.SqlClient;
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

    public async Task<Company?> GetByCodeAsync(int code)
    {
        return await _dbSet.FirstOrDefaultAsync(c => c.Code == code);
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

    public async Task<Company?> GetCompanyWithStoresByCodeAsync(int code)
    {
        return await _dbSet
            .Include(c => c.Stores)
            .FirstOrDefaultAsync(c => c.Code == code);
    }

    public override async Task AddAsync(Company entity)
    {
        var nameParam = new SqlParameter("@Name", entity.Name);
        var codeParam = new SqlParameter("@Code", entity.Code);
        var isActiveParam = new SqlParameter("@IsActive", entity.IsActive);
        var newIdParam = new SqlParameter("@NewId", System.Data.SqlDbType.UniqueIdentifier) { Direction = System.Data.ParameterDirection.Output };
        var errorMessageParam = new SqlParameter("@ErrorMessage", System.Data.SqlDbType.NVarChar, 500) { Direction = System.Data.ParameterDirection.Output };

        try
        {
            // Execute the stored procedure and capture return value
            var parameters = new[]
            {
                nameParam,
                codeParam,
                isActiveParam,
                newIdParam,
                errorMessageParam
            };

            var sql = @"
                DECLARE @ReturnValue INT;
                EXEC @ReturnValue = sp_InsertCompany @Name, @Code, @IsActive, @NewId OUTPUT, @ErrorMessage OUTPUT;
                SELECT @ReturnValue;";

            var queryResult = await _context.Database.SqlQueryRaw<int>(sql, parameters).ToListAsync();
            var returnValue = queryResult.FirstOrDefault();

            var errorMessage = errorMessageParam.Value?.ToString();

            switch (returnValue)
            {
                case 0: // Success
                    entity.Id = (Guid)newIdParam.Value;
                    entity.CreatedAt = DateTime.UtcNow;
                    entity.UpdatedAt = null;
                    break;
                case -1: // Validation error
                    throw new ArgumentException(errorMessage ?? "Validation error occurred");
                case -2: // Constraint violation
                    throw new InvalidOperationException(errorMessage ?? "Company code already exists");
                case -3: // General error
                default:
                    throw new Exception(errorMessage ?? "An error occurred while creating the company");
            }
        }
        catch (SqlException ex)
        {
            throw new Exception($"Database error occurred while creating company: {ex.Message}", ex);
        }
    }
}

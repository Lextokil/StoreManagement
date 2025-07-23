using Microsoft.Data.SqlClient;
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

    public override async Task AddAsync(Store entity)
    {
        var nameParam = new SqlParameter("@Name", entity.Name);
        var codeParam = new SqlParameter("@Code", entity.Code);
        var addressParam = new SqlParameter("@Address", (object?)entity.Address ?? DBNull.Value);
        var companyIdParam = new SqlParameter("@CompanyId", entity.CompanyId);
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
                addressParam,
                companyIdParam,
                isActiveParam,
                newIdParam,
                errorMessageParam
            };

            var sql = @"
                DECLARE @ReturnValue INT;
                EXEC @ReturnValue = sp_InsertStore @Name, @Code, @Address, @CompanyId, @IsActive, @NewId OUTPUT, @ErrorMessage OUTPUT;
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
                    throw new InvalidOperationException(errorMessage ?? "Store code already exists for this company");
                case -3: // General error
                default:
                    throw new Exception(errorMessage ?? "An error occurred while creating the store");
            }
        }
        catch (SqlException ex)
        {
            throw new Exception($"Database error occurred while creating store: {ex.Message}", ex);
        }
    }
}

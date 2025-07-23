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

    public override async Task<IEnumerable<Product>> GetAllAsync()
    {
        return await _dbSet
            .Include(p => p.Store)
            .ToListAsync();
    }

    public override async Task<Product?> GetByIdAsync(Guid id)
    {
        return await _dbSet
            .Include(p => p.Store)
            .FirstOrDefaultAsync(p => p.Id == id);
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

    public async Task<Product?> GetByCodesAsync(int companyCode, int storeCode, int productCode)
    {
        return await _dbSet
            .Include(p => p.Store)
            .ThenInclude(s => s.Company)
            .FirstOrDefaultAsync(p => p.Code == productCode && 
                                     p.Store.Code == storeCode && 
                                     p.Store.Company.Code == companyCode);
    }

    public async Task<string> GetProductsAsJsonAsync(Guid companyId)
    {
        var companyIdParam = new SqlParameter("@CompanyId", companyId);
        
        var result = await _context.Database
            .SqlQueryRaw<string>("SELECT dbo.GetProductsAsJson(@CompanyId) AS Value", companyIdParam)
            .FirstOrDefaultAsync();

        return result ?? "[]";
    }

    public override async Task AddAsync(Product entity)
    {
        var nameParam = new SqlParameter("@Name", entity.Name);
        var descriptionParam = new SqlParameter("@Description", (object?)entity.Description ?? DBNull.Value);
        var priceParam = new SqlParameter("@Price", entity.Price);
        var codeParam = new SqlParameter("@Code", entity.Code);
        var storeIdParam = new SqlParameter("@StoreId", entity.StoreId);
        var isActiveParam = new SqlParameter("@IsActive", entity.IsActive);
        var newIdParam = new SqlParameter("@NewId", System.Data.SqlDbType.UniqueIdentifier) { Direction = System.Data.ParameterDirection.Output };
        var errorMessageParam = new SqlParameter("@ErrorMessage", System.Data.SqlDbType.NVarChar, 500) { Direction = System.Data.ParameterDirection.Output };

        try
        {
            // Execute the stored procedure and capture return value
            var parameters = new[]
            {
                nameParam,
                descriptionParam,
                priceParam,
                codeParam,
                storeIdParam,
                isActiveParam,
                newIdParam,
                errorMessageParam
            };

            var sql = @"
                DECLARE @ReturnValue INT;
                EXEC @ReturnValue = sp_InsertProduct @Name, @Description, @Price, @Code, @StoreId, @IsActive, @NewId OUTPUT, @ErrorMessage OUTPUT;
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
                    throw new InvalidOperationException(errorMessage ?? "Product code already exists for this store");
                case -3: // General error
                default:
                    throw new Exception(errorMessage ?? "An error occurred while creating the product");
            }
        }
        catch (SqlException ex)
        {
            throw new Exception($"Database error occurred while creating product: {ex.Message}", ex);
        }
    }
}

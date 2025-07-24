using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StoreManagement.Database;
using StoreManagement.Domain.Interfaces;
using StoreManagement.Domain.Interfaces.Repositories;
using StoreManagement.Domain.Interfaces.Services;
using StoreManagement.Infrastructure.Data;
using StoreManagement.Infrastructure.Repositories;
using StoreManagement.Infrastructure.Services;

namespace StoreManagement.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

        services.AddScoped<ICompanyRepository, CompanyRepository>();
        services.AddScoped<IStoreRepository, StoreRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<ICompanyService, CompanyService>();
        services.AddScoped<IStoreService, StoreService>();
        services.AddScoped<IProductService, ProductService>();
        
        AddDataBase(services, configuration);
        
        // Add Database services (including migrations)
        services.AddMigrationService(configuration);

        return services;
    }

    private static void AddDataBase(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
                               ?? "Server=(localdb)\\mssqllocaldb;Database=StoreManagementDb;Trusted_Connection=true;MultipleActiveResultSets=true";

        services.AddDbContext<StoreManagementDbContext>(options =>
            options.UseSqlServer(connectionString));
    }
}

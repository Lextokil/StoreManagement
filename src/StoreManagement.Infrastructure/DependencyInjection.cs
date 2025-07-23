using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StoreManagement.Domain.Interfaces;
using StoreManagement.Domain.Interfaces.Repositories;
using StoreManagement.Domain.Interfaces.Services;
using StoreManagement.Infrastructure.Data;
using StoreManagement.Infrastructure.Repositories;
using StoreManagement.Infrastructure.Services;

namespace StoreManagement.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, WebApplicationBuilder builder)
    {
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

        services.AddScoped<ICompanyRepository, CompanyRepository>();
        services.AddScoped<IStoreRepository, StoreRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<ICompanyService, CompanyService>();
        services.AddScoped<IStoreService, StoreService>();
        services.AddScoped<IProductService, ProductService>();
        AddDataBase(builder);

        return services;
    }

    private static void AddDataBase(WebApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                               ?? "Server=(localdb)\\mssqllocaldb;Database=StoreManagementDb;Trusted_Connection=true;MultipleActiveResultSets=true";

        builder.Services.AddDbContext<StoreManagementDbContext>(options =>
            options.UseSqlServer(connectionString));
    }
}
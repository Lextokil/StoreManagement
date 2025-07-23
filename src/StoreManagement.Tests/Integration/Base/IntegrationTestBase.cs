using System.Reflection;
using FluentMigrator.Runner;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StoreManagement.API.Mappings;
using StoreManagement.Database.Migrations;
using StoreManagement.Domain.Interfaces;
using StoreManagement.Domain.Interfaces.Repositories;
using StoreManagement.Domain.Interfaces.Services;
using StoreManagement.Infrastructure;
using StoreManagement.Infrastructure.Data;
using StoreManagement.Infrastructure.Repositories;
using StoreManagement.Infrastructure.Services;

namespace StoreManagement.Tests.Integration.Base;

public abstract class IntegrationTestBase : IDisposable
{
    protected StoreManagementDbContext DbContext { get; private set; }
    protected IServiceProvider ServiceProvider { get; private set; }
    private readonly string _connectionString;
    private readonly string _databaseName;

    protected IntegrationTestBase()
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("testAppsettings.json", optional: false, reloadOnChange: true)
            .Build();

        _connectionString = configuration.GetConnectionString("TestConnection")
            ?? throw new InvalidOperationException("Test connection string not found.");

        var builder = new SqlConnectionStringBuilder(_connectionString);
        _databaseName = builder.InitialCatalog;

        SetupServices();
        SetupDatabase();
    }

    private void SetupDatabase()
    {
        // Ensure test database exists
        EnsureDatabaseExists();

        // Run migrations
        RunMigrations();

        // Clean existing data
        CleanDatabase().Wait();
    }

    private void EnsureDatabaseExists()
    {
        var builder = new SqlConnectionStringBuilder(_connectionString);
        builder.InitialCatalog = "master";
        var masterConnectionString = builder.ConnectionString;

        using var connection = new SqlConnection(masterConnectionString);
        connection.Open();

        var checkDbCommand = new SqlCommand($"SELECT COUNT(*) FROM sys.databases WHERE name = '{_databaseName}'", connection);
        var dbExists = (int)checkDbCommand.ExecuteScalar() > 0;

        if (!dbExists)
        {
            var createDbCommand = new SqlCommand($"CREATE DATABASE [{_databaseName}]", connection);
            createDbCommand.ExecuteNonQuery();
        }
    }

    private void RunMigrations()
    {
        var services = new ServiceCollection()
            .AddFluentMigratorCore()
            .ConfigureRunner(rb => rb
                .AddSqlServer()
                .WithGlobalConnectionString(_connectionString)
                .ScanIn(typeof(Migration001_CreateCompaniesTable).Assembly)
                .For.Migrations())
            .AddLogging(lb => lb.AddConsole());

        using var serviceProvider = services.BuildServiceProvider(false);
        using var scope = serviceProvider.CreateScope();
        
        var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
        runner.MigrateUp();
    }

    private void SetupServices()
    {
        var services = new ServiceCollection();

        // Add DbContext
        services.AddDbContext<StoreManagementDbContext>(options =>
            options.UseSqlServer(_connectionString));

        // Add AutoMapper
        services.AddAutoMapper(typeof(AutoMapperProfile));

        // Add repositories
        services.AddScoped<ICompanyRepository, CompanyRepository>();
        services.AddScoped<IStoreRepository, StoreRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Add services
        services.AddScoped<ICompanyService, CompanyService>();
        services.AddScoped<IStoreService, StoreService>();
        services.AddScoped<IProductService, ProductService>();

        // Add logging
        services.AddLogging();
        ServiceProvider = services.BuildServiceProvider();
        DbContext = ServiceProvider.GetRequiredService<StoreManagementDbContext>();
    }

    protected T GetService<T>() where T : class
    {
        return ServiceProvider.GetRequiredService<T>();
    }

    protected async Task AddEntities<T>(params T[] entities) where T : class
    {
        DbContext.Set<T>().AddRange(entities);
        await DbContext.SaveChangesAsync();
    }

    protected async Task AddEntities(params object[] entities)
    {
        foreach (var entity in entities)
        {
            DbContext.Add(entity);
        }
        await DbContext.SaveChangesAsync();
    }

    protected async Task CleanDatabase()
    {
        // Clean tables in correct order (respecting foreign keys)
        await DbContext.Database.ExecuteSqlRawAsync("DELETE FROM products");
        await DbContext.Database.ExecuteSqlRawAsync("DELETE FROM stores");
        await DbContext.Database.ExecuteSqlRawAsync("DELETE FROM companies");
    }

    public void Dispose()
    {
        DbContext?.Dispose();
        ServiceProvider?.GetService<IServiceScope>()?.Dispose();
    }

   
}

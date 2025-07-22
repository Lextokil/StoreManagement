using Azure.Identity;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StoreManagement.Domain.Interfaces;
using StoreManagement.Domain.Interfaces.Repositories;
using StoreManagement.Domain.Interfaces.Services;
using StoreManagement.Infrastructure.Data;
using StoreManagement.Infrastructure.Repositories;
using StoreManagement.Infrastructure.Services;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureAppConfiguration((context, config) =>
    {
        // Add configuration sources
        config.AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
              .AddEnvironmentVariables();

        // Configure Azure App Configuration and Key Vault
        var keyVaultEndpoint = config.Build()["KeyVaultEndpoint"];
        var appConfigEndpoint = config.Build()["AppConfigEndpoint"];

        if (!string.IsNullOrEmpty(appConfigEndpoint))
        {
            config.AddAzureAppConfiguration(options =>
            {
                options.Connect(appConfigEndpoint)
                       .ConfigureKeyVault(kv =>
                       {
                           if (!string.IsNullOrEmpty(keyVaultEndpoint))
                           {
                               kv.SetCredential(new DefaultAzureCredential());
                           }
                       });
            });
        }

        if (!string.IsNullOrEmpty(keyVaultEndpoint))
        {
            config.AddAzureKeyVault(
                new Uri(keyVaultEndpoint),
                new DefaultAzureCredential());
        }
    })
    .ConfigureServices((context, services) =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();

        // Configure Entity Framework
        var connectionString = context.Configuration.GetConnectionString("DefaultConnection")
            ?? "Server=(localdb)\\mssqllocaldb;Database=StoreManagementDb;Trusted_Connection=true;MultipleActiveResultSets=true";

        services.AddDbContext<StoreManagementDbContext>(options =>
            options.UseSqlServer(connectionString));

        // Register repositories
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

        // Register services
        services.AddScoped<IProductService, ProductService>();

        // Add AutoMapper
        services.AddAutoMapper(typeof(Program));
    })
    .Build();

host.Run();

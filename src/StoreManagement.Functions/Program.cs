using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StoreManagement.Domain.Interfaces.Services;
using StoreManagement.Functions;
using StoreManagement.Infrastructure;
using StoreManagement.Infrastructure.Mappings;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureAppConfiguration((context, config) =>
    {
        config.AddAzureConfiguration();
    })
    .ConfigureServices((context, services) =>
    {
        services.AddInfrastructure(context.Configuration)
                .AddFunctions();

        services.AddAutoMapper(typeof(AutoMapperProfile));
    })
    .Build();

// Run database migrations automatically
using (var scope = host.Services.CreateScope())
{
    var migrationService = scope.ServiceProvider.GetRequiredService<IMigrationService>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    
    try
    {
        logger.LogInformation("Starting database migrations...");
        await migrationService.RunMigrationsAsync();
        logger.LogInformation("Database migrations completed successfully.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Failed to run database migrations. Functions will continue but may not work properly.");
        // Continue running the functions even if migrations fail
        // This allows for debugging and manual intervention
    }
}

host.Run();

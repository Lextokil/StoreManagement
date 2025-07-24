using Azure.Identity;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace StoreManagement.Functions;

public static class DependencyInjection
{
    public static IServiceCollection AddFunctions(this IServiceCollection services)
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();

        return services;
    }

    public static IConfigurationBuilder AddAzureConfiguration(this IConfigurationBuilder config)
    {
        // Add configuration sources
        config.AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
              .AddEnvironmentVariables();

        // Configure Azure App Configuration and Key Vault
        var configuration = config.Build();
        var keyVaultEndpoint = configuration["KeyVaultEndpoint"];
        var appConfigEndpoint = configuration["AppConfigEndpoint"];

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

        return config;
    }
}

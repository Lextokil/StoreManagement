using FluentMigrator.Runner;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace StoreManagement.Database;

class Program
{
    static void Main(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        // Create database if it doesn't exist
        EnsureDatabaseExists(connectionString);

        var host = Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((context, config) =>
            {
                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            })
            .ConfigureServices((context, services) =>
            {
                services.AddFluentMigratorCore()
                    .ConfigureRunner(rb => rb
                        .AddSqlServer()
                        .WithGlobalConnectionString(connectionString)
                        .ScanIn(typeof(Program).Assembly).For.Migrations())
                    .AddLogging(lb => lb.AddFluentMigratorConsole());
            })
            .Build();

        using var scope = host.Services.CreateScope();
        var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

        try
        {
            if (args.Contains("--rollback"))
            {
                logger.LogInformation("Rolling back all migrations...");
                runner.RollbackToVersion(0);
                logger.LogInformation("Rollback completed successfully.");
            }
            else
            {
                logger.LogInformation("Running migrations...");
                runner.MigrateUp();
                logger.LogInformation("Migrations completed successfully.");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while running migrations.");
            throw;
        }
    }

    private static void EnsureDatabaseExists(string connectionString)
    {
        var builder = new SqlConnectionStringBuilder(connectionString);
        var databaseName = builder.InitialCatalog;
        
        // Remove database name from connection string to connect to master
        builder.InitialCatalog = "master";
        var masterConnectionString = builder.ConnectionString;

        using var connection = new SqlConnection(masterConnectionString);
        connection.Open();

        // Check if database exists
        var checkDbCommand = new SqlCommand($"SELECT COUNT(*) FROM sys.databases WHERE name = '{databaseName}'", connection);
        var dbExists = (int)checkDbCommand.ExecuteScalar() > 0;

        if (!dbExists)
        {
            Console.WriteLine($"Creating database '{databaseName}'...");
            var createDbCommand = new SqlCommand($"CREATE DATABASE [{databaseName}]", connection);
            createDbCommand.ExecuteNonQuery();
            Console.WriteLine($"Database '{databaseName}' created successfully.");
        }
        else
        {
            Console.WriteLine($"Database '{databaseName}' already exists.");
        }
    }
}

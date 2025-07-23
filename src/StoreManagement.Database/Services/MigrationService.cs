using FluentMigrator.Runner;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StoreManagement.Domain.Interfaces.Services;

namespace StoreManagement.Database.Services;

public class MigrationService : IMigrationService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;
    private readonly ILogger<MigrationService> _logger;

    public MigrationService(IServiceProvider serviceProvider, IConfiguration configuration, ILogger<MigrationService> logger)
    {
        _serviceProvider = serviceProvider;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task RunMigrationsAsync()
    {
        var connectionString = GetConnectionString();
        
        // Create database if it doesn't exist
        await EnsureDatabaseExistsAsync(connectionString);

        using var scope = _serviceProvider.CreateScope();
        var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();

        try
        {
            _logger.LogInformation("Running migrations...");
            runner.MigrateUp();
            _logger.LogInformation("Migrations completed successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while running migrations.");
            throw;
        }
    }

    public async Task RollbackMigrationsAsync(long version = 0)
    {
        using var scope = _serviceProvider.CreateScope();
        var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();

        try
        {
            _logger.LogInformation("Rolling back migrations to version {Version}...", version);
            runner.RollbackToVersion(version);
            _logger.LogInformation("Rollback completed successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while rolling back migrations.");
            throw;
        }

        await Task.CompletedTask;
    }

    public async Task<bool> HasPendingMigrationsAsync()
    {
        // For FluentMigrator, we'll assume there might be pending migrations
        // This method can be enhanced later if needed with more specific logic
        await Task.CompletedTask;
        return true;
    }

    private string GetConnectionString()
    {
        return _configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
    }

    private async Task EnsureDatabaseExistsAsync(string connectionString)
    {
        var builder = new SqlConnectionStringBuilder(connectionString);
        var databaseName = builder.InitialCatalog;
        
        // Remove database name from connection string to connect to master
        builder.InitialCatalog = "master";
        var masterConnectionString = builder.ConnectionString;

        using var connection = new SqlConnection(masterConnectionString);
        await connection.OpenAsync();

        // Check if database exists
        var checkDbCommand = new SqlCommand($"SELECT COUNT(*) FROM sys.databases WHERE name = '{databaseName}'", connection);
        var dbExists = (int)(await checkDbCommand.ExecuteScalarAsync()) > 0;

        if (!dbExists)
        {
            _logger.LogInformation("Creating database '{DatabaseName}'...", databaseName);
            var createDbCommand = new SqlCommand($"CREATE DATABASE [{databaseName}]", connection);
            await createDbCommand.ExecuteNonQueryAsync();
            _logger.LogInformation("Database '{DatabaseName}' created successfully.", databaseName);
        }
        else
        {
            _logger.LogDebug("Database '{DatabaseName}' already exists.", databaseName);
        }
    }
}

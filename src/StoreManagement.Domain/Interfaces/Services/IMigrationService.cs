namespace StoreManagement.Domain.Interfaces.Services;

public interface IMigrationService
{
    Task RunMigrationsAsync();
    Task RollbackMigrationsAsync(long version = 0);
    Task<bool> HasPendingMigrationsAsync();
}

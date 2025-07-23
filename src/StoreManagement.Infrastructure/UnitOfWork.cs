using Microsoft.EntityFrameworkCore;
using StoreManagement.Domain.Entities;
using StoreManagement.Domain.Interfaces;
using StoreManagement.Infrastructure.Data;

namespace StoreManagement.Infrastructure;

public class UnitOfWork : IUnitOfWork
{
    private readonly StoreManagementDbContext _context;

    public UnitOfWork(StoreManagementDbContext context)
    {
        _context = context;
    }

    public async Task<int> CommitAsync()
    {
        AddAuditInformation();

        var rows = await _context.SaveChangesAsync();
        return rows;
    }

    public Task RollbackSync()
    {
        foreach (var entry in _context.ChangeTracker.Entries().ToList())
        {
            switch (entry.State)
            {
                case EntityState.Modified:
                    entry.CurrentValues.SetValues(entry.OriginalValues);
                    entry.State = EntityState.Unchanged;
                    break;

                case EntityState.Added:
                    entry.State = EntityState.Detached;
                    break;

                case EntityState.Deleted:
                    entry.State = EntityState.Unchanged;
                    break;
            }
        }

        return Task.CompletedTask;
    }

    private void AddAuditInformation()
    {
        var entries = _context.ChangeTracker.Entries().Where(entry => entry.Entity is BaseEntity)
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

        foreach (var entry in entries)
        {
            var createdAtProperty = entry.Property("CreatedAt");
            if (createdAtProperty != null && entry.State == EntityState.Added)
            {
                createdAtProperty.CurrentValue = DateTime.UtcNow;
            }

            var updatedAtProperty = entry.Properties.FirstOrDefault(p => p.Metadata.Name == "UpdatedAt");
            if (updatedAtProperty != null && entry.State == EntityState.Modified)
            {
                updatedAtProperty.CurrentValue = DateTime.UtcNow;
            }
        }
    }
}

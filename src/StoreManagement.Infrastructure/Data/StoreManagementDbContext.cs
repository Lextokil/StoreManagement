using Microsoft.EntityFrameworkCore;
using StoreManagement.Domain.Entities;

namespace StoreManagement.Infrastructure.Data;

public class StoreManagementDbContext : DbContext
{
    public StoreManagementDbContext(DbContextOptions<StoreManagementDbContext> options) : base(options)
    {
    }

    public DbSet<Company> Companies { get; set; }
    public DbSet<Store> Stores { get; set; }
    public DbSet<Product> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(StoreManagement.Domain.Mappings.CompanyMapping).Assembly);
        
        SeedData(modelBuilder);
    }
    
    private void SeedData(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Company>().HasData(
            new Company
            {
                Id = Guid.NewGuid(),
                Name = "Demo Company",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            }
        );
    }
}

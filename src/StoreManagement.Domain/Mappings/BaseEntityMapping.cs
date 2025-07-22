using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StoreManagement.Domain.Entities;

namespace StoreManagement.Domain.Mappings;

public abstract class BaseEntityMapping<T> : IEntityTypeConfiguration<T> 
    where T : BaseEntity
{
    public virtual void Configure(EntityTypeBuilder<T> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        
        builder.Property(x => x.IsActive)
            .HasDefaultValue(true)
            .HasColumnName("is_active");
            
        builder.Property(x => x.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()")
            .HasColumnName("created_at");
            
        builder.Property(x => x.UpdatedAt)
            .IsRequired(false)
            .HasColumnName("updated_at");
        
        builder.HasIndex(x => x.IsActive)
            .HasDatabaseName($"IX_{typeof(T).Name}_IsActive");
        builder.HasIndex(x => x.CreatedAt)
            .HasDatabaseName($"IX_{typeof(T).Name}_CreatedAt");
    }
}

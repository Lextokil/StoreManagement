using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StoreManagement.Domain.Entities;

namespace StoreManagement.Domain.Mappings;

public class StoreMapping : BaseEntityMapping<Store>
{
    public override void Configure(EntityTypeBuilder<Store> builder)
    {
        base.Configure(builder);
        
        builder.ToTable("stores");
        
        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(255)
            .HasColumnName("name");
            
        builder.Property(x => x.Code)
            .IsRequired()
            .HasColumnName("code");
            
        builder.Property(x => x.Address)
            .HasMaxLength(500)
            .HasColumnName("address");
            
        builder.Property(x => x.CompanyId)
            .IsRequired()
            .HasColumnName("company_id");
            
        // Unique constraint for code + company_id
        builder.HasIndex(x => new { x.Code, x.CompanyId })
            .IsUnique()
            .HasDatabaseName("IX_Store_Code_CompanyId_Unique");
            
        // Foreign key index
        builder.HasIndex(x => x.CompanyId)
            .HasDatabaseName("IX_Store_CompanyId");
            
        // Navigation properties
        builder.HasOne(x => x.Company)
            .WithMany(x => x.Stores)
            .HasForeignKey(x => x.CompanyId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasMany(x => x.Products)
            .WithOne(x => x.Store)
            .HasForeignKey(x => x.StoreId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StoreManagement.Domain.Entities;

namespace StoreManagement.Domain.Mappings;

public class StoreMapping : BaseEntityMapping<Store>
{
    public override void Configure(EntityTypeBuilder<Store> builder)
    {
        base.Configure(builder);
        
        builder.ToTable("Stores");
        
        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100)
            .HasColumnName("name");
            
        builder.Property(x => x.Address)
            .HasMaxLength(200)
            .HasColumnName("address")
            .IsRequired(false);
            
        builder.Property(x => x.CompanyId)
            .IsRequired()
            .HasColumnName("company_id");
            
        builder.HasOne(x => x.Company)
            .WithMany(x => x.Stores)
            .HasForeignKey(x => x.CompanyId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasMany(x => x.Products)
            .WithOne(x => x.Store)
            .HasForeignKey(x => x.StoreId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasIndex(x => x.CompanyId)
            .HasDatabaseName("IX_Stores_CompanyId");
        builder.HasIndex(x => x.Name)
            .HasDatabaseName("IX_Stores_Name");
    }
}

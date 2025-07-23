using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StoreManagement.Domain.Entities;

namespace StoreManagement.Domain.Mappings;

public class ProductMapping : BaseEntityMapping<Product>
{
    public override void Configure(EntityTypeBuilder<Product> builder)
    {
        base.Configure(builder);
        
        builder.ToTable("products");
        
        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(255)
            .HasColumnName("name");
            
        builder.Property(x => x.Description)
            .HasMaxLength(1000)
            .HasColumnName("description");
            
        builder.Property(x => x.Price)
            .IsRequired()
            .HasColumnType("decimal(18,2)")
            .HasColumnName("price");
            
        builder.Property(x => x.Code)
            .IsRequired()
            .HasColumnName("code");
            
        builder.Property(x => x.StoreId)
            .IsRequired()
            .HasColumnName("store_id");
            
        // Unique constraint for code + store_id
        builder.HasIndex(x => new { x.Code, x.StoreId })
            .IsUnique()
            .HasDatabaseName("IX_Product_Code_StoreId_Unique");
            
        // Foreign key index
        builder.HasIndex(x => x.StoreId)
            .HasDatabaseName("IX_Product_StoreId");
            
        // Navigation property
        builder.HasOne(x => x.Store)
            .WithMany(x => x.Products)
            .HasForeignKey(x => x.StoreId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

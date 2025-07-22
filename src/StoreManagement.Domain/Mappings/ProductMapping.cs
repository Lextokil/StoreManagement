using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StoreManagement.Domain.Entities;

namespace StoreManagement.Domain.Mappings;

public class ProductMapping : BaseEntityMapping<Product>
{
    public override void Configure(EntityTypeBuilder<Product> builder)
    {
        base.Configure(builder);
        
        builder.ToTable("Products");
        
        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100)
            .HasColumnName("name");
            
        builder.Property(x => x.Description)
            .HasMaxLength(500)
            .HasColumnName("description")
            .IsRequired(false);
            
        builder.Property(x => x.Price)
            .HasColumnType("decimal(18,2)")
            .HasColumnName("price")
            .IsRequired();
            
        builder.Property(x => x.StockQuantity)
            .HasColumnName("stock_quantity")
            .HasDefaultValue(0);
            
        builder.Property(x => x.StoreId)
            .IsRequired()
            .HasColumnName("store_id");
            
        builder.HasOne(x => x.Store)
            .WithMany(x => x.Products)
            .HasForeignKey(x => x.StoreId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasIndex(x => x.StoreId)
            .HasDatabaseName("IX_Products_StoreId");
        builder.HasIndex(x => x.Name)
            .HasDatabaseName("IX_Products_Name");
        builder.HasIndex(x => x.Price)
            .HasDatabaseName("IX_Products_Price");
        builder.HasIndex(x => x.StockQuantity)
            .HasDatabaseName("IX_Products_StockQuantity");
    }
}

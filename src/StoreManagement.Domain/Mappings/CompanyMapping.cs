using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StoreManagement.Domain.Entities;

namespace StoreManagement.Domain.Mappings;

public class CompanyMapping : BaseEntityMapping<Company>
{
    public override void Configure(EntityTypeBuilder<Company> builder)
    {
        base.Configure(builder);
        
        builder.ToTable("companies");
        
        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(255)
            .HasColumnName("name");
            
        builder.Property(x => x.Code)
            .IsRequired()
            .HasColumnName("code");
            
        // Unique constraint for code
        builder.HasIndex(x => x.Code)
            .IsUnique()
            .HasDatabaseName("IX_Company_Code_Unique");
            
        // Navigation property
        builder.HasMany(x => x.Stores)
            .WithOne(x => x.Company)
            .HasForeignKey(x => x.CompanyId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

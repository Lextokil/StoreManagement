using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StoreManagement.Domain.Entities;

namespace StoreManagement.Domain.Mappings;

public class CompanyMapping : BaseEntityMapping<Company>
{
    public override void Configure(EntityTypeBuilder<Company> builder)
    {
        base.Configure(builder);
        
        builder.ToTable("Companies");
        
        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100)
            .HasColumnName("name");
            
        builder.HasMany(x => x.Stores)
            .WithOne(x => x.Company)
            .HasForeignKey(x => x.CompanyId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasIndex(x => x.Name)
            .IsUnique()
            .HasDatabaseName("IX_Companies_Name_Unique");
    }
}

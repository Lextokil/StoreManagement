using FluentMigrator;

namespace StoreManagement.Database.Migrations;

[Migration(202501210003, "Create Products Table")]
public class Migration003_CreateProductsTable : Migration
{
    public override void Up()
    {
        Create.Table("Products")
            .WithColumn("Id").AsInt32().PrimaryKey().Identity()
            .WithColumn("Name").AsString(100).NotNullable()
            .WithColumn("Description").AsString(500).Nullable()
            .WithColumn("Price").AsDecimal(18, 2).NotNullable().WithDefaultValue(0)
            .WithColumn("StockQuantity").AsInt32().NotNullable().WithDefaultValue(0)
            .WithColumn("IsActive").AsBoolean().NotNullable().WithDefaultValue(true)
            .WithColumn("CreatedAt").AsDateTime2().NotNullable().WithDefaultValue(SystemMethods.CurrentUTCDateTime)
            .WithColumn("StoreId").AsInt32().NotNullable();

        // Create foreign key constraint
        Create.ForeignKey("FK_Products_Stores")
            .FromTable("Products").ForeignColumn("StoreId")
            .ToTable("Stores").PrimaryColumn("Id")
            .OnDelete(System.Data.Rule.Cascade);

        // Create indexes for better performance
        Create.Index("IX_Products_StoreId").OnTable("Products").OnColumn("StoreId");
        Create.Index("IX_Products_Name").OnTable("Products").OnColumn("Name");
    }

    public override void Down()
    {
        Delete.Table("Products");
    }
}

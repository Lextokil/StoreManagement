using FluentMigrator;

namespace StoreManagement.Database.Migrations;

[Migration(003)]
public class Migration003_CreateProductsTable : Migration
{
    public override void Up()
    {
        Create.Table("products")
            .WithColumn("id").AsGuid().PrimaryKey().NotNullable()
            .WithColumn("name").AsString(255).NotNullable()
            .WithColumn("description").AsString(1000).Nullable()
            .WithColumn("price").AsDecimal(18, 2).NotNullable()
            .WithColumn("code").AsInt32().NotNullable()
            .WithColumn("store_id").AsGuid().NotNullable()
            .WithColumn("is_active").AsBoolean().NotNullable().WithDefaultValue(true)
            .WithColumn("created_at").AsDateTime2().NotNullable().WithDefaultValue(SystemMethods.CurrentUTCDateTime)
            .WithColumn("updated_at").AsDateTime2().Nullable();

        Create.ForeignKey("FK_Products_Stores")
            .FromTable("products").ForeignColumn("store_id")
            .ToTable("stores").PrimaryColumn("id")
            .OnDelete(System.Data.Rule.Cascade);

        Create.UniqueConstraint("UQ_Products_Code_StoreId")
            .OnTable("products")
            .Columns("code", "store_id");

        Create.Index("IX_Product_StoreId")
            .OnTable("products")
            .OnColumn("store_id");

        Create.Index("IX_Product_IsActive")
            .OnTable("products")
            .OnColumn("is_active");

        Create.Index("IX_Product_CreatedAt")
            .OnTable("products")
            .OnColumn("created_at");

        Create.Index("IX_Product_Name")
            .OnTable("products")
            .OnColumn("name");

        Create.Index("IX_Product_Price")
            .OnTable("products")
            .OnColumn("price");
    }

    public override void Down()
    {
        Delete.Table("products");
    }
}

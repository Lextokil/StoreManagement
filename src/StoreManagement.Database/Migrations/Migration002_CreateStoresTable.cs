using FluentMigrator;

namespace StoreManagement.Database.Migrations;

[Migration(002)]
public class Migration002_CreateStoresTable : Migration
{
    public override void Up()
    {
        Create.Table("stores")
            .WithColumn("id").AsGuid().PrimaryKey().NotNullable()
            .WithColumn("name").AsString(255).NotNullable()
            .WithColumn("code").AsInt32().NotNullable()
            .WithColumn("address").AsString(500).Nullable()
            .WithColumn("company_id").AsGuid().NotNullable()
            .WithColumn("is_active").AsBoolean().NotNullable().WithDefaultValue(true)
            .WithColumn("created_at").AsDateTime2().NotNullable().WithDefaultValue(SystemMethods.CurrentUTCDateTime)
            .WithColumn("updated_at").AsDateTime2().Nullable();

        Create.ForeignKey("FK_Stores_Companies")
            .FromTable("stores").ForeignColumn("company_id")
            .ToTable("companies").PrimaryColumn("id")
            .OnDelete(System.Data.Rule.Cascade);

        Create.UniqueConstraint("UQ_Stores_Code_CompanyId")
            .OnTable("stores")
            .Columns("code", "company_id");

        Create.Index("IX_Store_CompanyId")
            .OnTable("stores")
            .OnColumn("company_id");

        Create.Index("IX_Store_IsActive")
            .OnTable("stores")
            .OnColumn("is_active");

        Create.Index("IX_Store_CreatedAt")
            .OnTable("stores")
            .OnColumn("created_at");
    }

    public override void Down()
    {
        Delete.Table("stores");
    }
}

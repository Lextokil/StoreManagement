using FluentMigrator;

namespace StoreManagement.Database.Migrations;

[Migration(001)]
public class Migration001_CreateCompaniesTable : Migration
{
    public override void Up()
    {
        Create.Table("companies")
            .WithColumn("id").AsGuid().PrimaryKey().NotNullable()
            .WithColumn("name").AsString(255).NotNullable()
            .WithColumn("code").AsInt32().NotNullable()
            .WithColumn("is_active").AsBoolean().NotNullable().WithDefaultValue(true)
            .WithColumn("created_at").AsDateTime2().NotNullable().WithDefaultValue(SystemMethods.CurrentUTCDateTime)
            .WithColumn("updated_at").AsDateTime2().Nullable();

        Create.UniqueConstraint("UQ_Companies_Code")
            .OnTable("companies")
            .Column("code");

        Create.Index("IX_Company_IsActive")
            .OnTable("companies")
            .OnColumn("is_active");

        Create.Index("IX_Company_CreatedAt")
            .OnTable("companies")
            .OnColumn("created_at");
    }

    public override void Down()
    {
        Delete.Table("companies");
    }
}

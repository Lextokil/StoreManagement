using FluentMigrator;

namespace StoreManagement.Database.Migrations;

[Migration(202501210001, "Create Companies Table")]
public class Migration001_CreateCompaniesTable : Migration
{
    public override void Up()
    {
        Create.Table("Companies")
            .WithColumn("Id").AsInt32().PrimaryKey().Identity()
            .WithColumn("Name").AsString(100).NotNullable()
            .WithColumn("IsActive").AsBoolean().NotNullable().WithDefaultValue(true)
            .WithColumn("CreatedAt").AsDateTime2().NotNullable().WithDefaultValue(SystemMethods.CurrentUTCDateTime);

        // Create index on Name for better performance
        Create.Index("IX_Companies_Name").OnTable("Companies").OnColumn("Name");

        // Insert seed data
        Insert.IntoTable("Companies").Row(new { 
            Name = "Demo Company", 
            IsActive = true 
        });
    }

    public override void Down()
    {
        Delete.Table("Companies");
    }
}

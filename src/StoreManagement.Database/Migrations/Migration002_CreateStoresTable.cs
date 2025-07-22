using FluentMigrator;

namespace StoreManagement.Database.Migrations;

[Migration(202501210002, "Create Stores Table")]
public class Migration002_CreateStoresTable : Migration
{
    public override void Up()
    {
        Create.Table("Stores")
            .WithColumn("Id").AsInt32().PrimaryKey().Identity()
            .WithColumn("Name").AsString(100).NotNullable()
            .WithColumn("Address").AsString(200).Nullable()
            .WithColumn("IsActive").AsBoolean().NotNullable().WithDefaultValue(true)
            .WithColumn("CreatedAt").AsDateTime2().NotNullable().WithDefaultValue(SystemMethods.CurrentUTCDateTime)
            .WithColumn("CompanyId").AsInt32().NotNullable();

        // Create foreign key constraint
        Create.ForeignKey("FK_Stores_Companies")
            .FromTable("Stores").ForeignColumn("CompanyId")
            .ToTable("Companies").PrimaryColumn("Id")
            .OnDelete(System.Data.Rule.Cascade);

        // Create indexes for better performance
        Create.Index("IX_Stores_CompanyId").OnTable("Stores").OnColumn("CompanyId");
        Create.Index("IX_Stores_Name").OnTable("Stores").OnColumn("Name");
    }

    public override void Down()
    {
        Delete.Table("Stores");
    }
}

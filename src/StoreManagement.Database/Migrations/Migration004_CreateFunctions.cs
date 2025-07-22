using FluentMigrator;

namespace StoreManagement.Database.Migrations;

[Migration(202501210004, "Create SQL Functions")]
public class Migration004_CreateFunctions : Migration
{
    public override void Up()
    {
        // Scalar function to return products as JSON for a specific company
        Execute.Sql(@"
            CREATE OR ALTER FUNCTION dbo.GetProductsAsJson(@CompanyId INT)
            RETURNS NVARCHAR(MAX)
            AS
            BEGIN
                DECLARE @JsonResult NVARCHAR(MAX);
                
                SELECT @JsonResult = (
                    SELECT 
                        p.Id,
                        p.Name,
                        p.Description,
                        p.Price,
                        p.StockQuantity,
                        p.IsActive,
                        p.CreatedAt,
                        p.StoreId,
                        s.Name AS StoreName
                    FROM Products p
                    INNER JOIN Stores s ON p.StoreId = s.Id
                    INNER JOIN Companies c ON s.CompanyId = c.Id
                    WHERE c.Id = @CompanyId
                    FOR JSON PATH
                );
                
                RETURN ISNULL(@JsonResult, '[]');
            END;
        ");

        // Scalar function to get product count by company
        Execute.Sql(@"
            CREATE OR ALTER FUNCTION dbo.GetProductCountByCompany(@CompanyId INT)
            RETURNS INT
            AS
            BEGIN
                DECLARE @Count INT;
                
                SELECT @Count = COUNT(*)
                FROM Products p
                INNER JOIN Stores s ON p.StoreId = s.Id
                WHERE s.CompanyId = @CompanyId AND p.IsActive = 1;
                
                RETURN ISNULL(@Count, 0);
            END;
        ");

        // Scalar function to get store count by company
        Execute.Sql(@"
            CREATE OR ALTER FUNCTION dbo.GetStoreCountByCompany(@CompanyId INT)
            RETURNS INT
            AS
            BEGIN
                DECLARE @Count INT;
                
                SELECT @Count = COUNT(*)
                FROM Stores
                WHERE CompanyId = @CompanyId AND IsActive = 1;
                
                RETURN ISNULL(@Count, 0);
            END;
        ");

        // Scalar function to calculate total inventory value by company
        Execute.Sql(@"
            CREATE OR ALTER FUNCTION dbo.GetTotalInventoryValueByCompany(@CompanyId INT)
            RETURNS DECIMAL(18,2)
            AS
            BEGIN
                DECLARE @TotalValue DECIMAL(18,2);
                
                SELECT @TotalValue = SUM(p.Price * p.StockQuantity)
                FROM Products p
                INNER JOIN Stores s ON p.StoreId = s.Id
                WHERE s.CompanyId = @CompanyId AND p.IsActive = 1;
                
                RETURN ISNULL(@TotalValue, 0);
            END;
        ");

        // Scalar function to get low stock products count
        Execute.Sql(@"
            CREATE OR ALTER FUNCTION dbo.GetLowStockProductsCount(@CompanyId INT)
            RETURNS INT
            AS
            BEGIN
                DECLARE @Count INT;
                
                -- Count products with stock quantity <= 10 as 'low stock'
                SELECT @Count = COUNT(*)
                FROM Products p
                INNER JOIN Stores s ON p.StoreId = s.Id
                WHERE s.CompanyId = @CompanyId 
                  AND p.IsActive = 1
                  AND p.StockQuantity <= 10;
                
                RETURN ISNULL(@Count, 0);
            END;
        ");
    }

    public override void Down()
    {
        Execute.Sql("DROP FUNCTION IF EXISTS dbo.GetProductsAsJson");
        Execute.Sql("DROP FUNCTION IF EXISTS dbo.GetProductCountByCompany");
        Execute.Sql("DROP FUNCTION IF EXISTS dbo.GetStoreCountByCompany");
        Execute.Sql("DROP FUNCTION IF EXISTS dbo.GetTotalInventoryValueByCompany");
        Execute.Sql("DROP FUNCTION IF EXISTS dbo.GetLowStockProductsCount");
    }
}

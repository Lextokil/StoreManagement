using FluentMigrator;

namespace StoreManagement.Database.Migrations;

[Migration(202501210005, "Create Stored Procedures")]
public class Migration005_CreateProcedures : Migration
{
    public override void Up()
    {
        // Procedure to insert a new product
        Execute.Sql(@"
            CREATE OR ALTER PROCEDURE dbo.InsertProduct
                @Name NVARCHAR(100),
                @Description NVARCHAR(500) = NULL,
                @Price DECIMAL(18,2) = 0,
                @StockQuantity INT = 0,
                @IsActive BIT = 1,
                @StoreId INT,
                @ProductId INT OUTPUT
            AS
            BEGIN
                SET NOCOUNT ON;
                
                BEGIN TRY
                    -- Validate store exists
                    IF NOT EXISTS (SELECT 1 FROM Stores WHERE Id = @StoreId)
                    BEGIN
                        RAISERROR('Store with ID %d does not exist.', 16, 1, @StoreId);
                        RETURN;
                    END
                    
                    -- Insert the product
                    INSERT INTO Products (
                        Name, Description, Price, StockQuantity, IsActive, StoreId, CreatedAt
                    )
                    VALUES (
                        @Name, @Description, @Price, @StockQuantity, @IsActive, @StoreId, GETUTCDATE()
                    );
                    
                    SET @ProductId = SCOPE_IDENTITY();
                    
                    PRINT 'Product inserted successfully with ID: ' + CAST(@ProductId AS NVARCHAR(10));
                    
                END TRY
                BEGIN CATCH
                    DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
                    DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
                    DECLARE @ErrorState INT = ERROR_STATE();
                    
                    RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
                END CATCH
            END;
        ");

        // Procedure to insert a new store
        Execute.Sql(@"
            CREATE OR ALTER PROCEDURE dbo.InsertStore
                @Name NVARCHAR(100),
                @Address NVARCHAR(200) = NULL,
                @IsActive BIT = 1,
                @CompanyId INT,
                @StoreId INT OUTPUT
            AS
            BEGIN
                SET NOCOUNT ON;
                
                BEGIN TRY
                    -- Validate company exists
                    IF NOT EXISTS (SELECT 1 FROM Companies WHERE Id = @CompanyId)
                    BEGIN
                        RAISERROR('Company with ID %d does not exist.', 16, 1, @CompanyId);
                        RETURN;
                    END
                    
                    -- Insert the store
                    INSERT INTO Stores (
                        Name, Address, IsActive, CompanyId, CreatedAt
                    )
                    VALUES (
                        @Name, @Address, @IsActive, @CompanyId, GETUTCDATE()
                    );
                    
                    SET @StoreId = SCOPE_IDENTITY();
                    
                    PRINT 'Store inserted successfully with ID: ' + CAST(@StoreId AS NVARCHAR(10));
                    
                END TRY
                BEGIN CATCH
                    DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
                    DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
                    DECLARE @ErrorState INT = ERROR_STATE();
                    
                    RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
                END CATCH
            END;
        ");

        // Procedure to insert a new company
        Execute.Sql(@"
            CREATE OR ALTER PROCEDURE dbo.InsertCompany
                @Name NVARCHAR(100),
                @IsActive BIT = 1,
                @CompanyId INT OUTPUT
            AS
            BEGIN
                SET NOCOUNT ON;
                
                BEGIN TRY
                    -- Insert the company
                    INSERT INTO Companies (Name, IsActive, CreatedAt)
                    VALUES (@Name, @IsActive, GETUTCDATE());
                    
                    SET @CompanyId = SCOPE_IDENTITY();
                    
                    PRINT 'Company inserted successfully with ID: ' + CAST(@CompanyId AS NVARCHAR(10));
                    
                END TRY
                BEGIN CATCH
                    DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
                    DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
                    DECLARE @ErrorState INT = ERROR_STATE();
                    
                    RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
                END CATCH
            END;
        ");

        // Procedure to update product stock
        Execute.Sql(@"
            CREATE OR ALTER PROCEDURE dbo.UpdateProductStock
                @ProductId INT,
                @NewStockQuantity INT,
                @UpdatedBy NVARCHAR(100) = 'System'
            AS
            BEGIN
                SET NOCOUNT ON;
                
                BEGIN TRY
                    -- Validate product exists
                    IF NOT EXISTS (SELECT 1 FROM Products WHERE Id = @ProductId)
                    BEGIN
                        RAISERROR('Product with ID %d does not exist.', 16, 1, @ProductId);
                        RETURN;
                    END
                    
                    -- Update stock quantity
                    UPDATE Products 
                    SET StockQuantity = @NewStockQuantity
                    WHERE Id = @ProductId;
                    
                    PRINT 'Product stock updated successfully for Product ID: ' + CAST(@ProductId AS NVARCHAR(10));
                    
                END TRY
                BEGIN CATCH
                    DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
                    DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
                    DECLARE @ErrorState INT = ERROR_STATE();
                    
                    RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
                END CATCH
            END;
        ");

        // Procedure to get company dashboard data
        Execute.Sql(@"
            CREATE OR ALTER PROCEDURE dbo.GetCompanyDashboard
                @CompanyId INT
            AS
            BEGIN
                SET NOCOUNT ON;
                
                SELECT 
                    c.Id AS CompanyId,
                    c.Name AS CompanyName,
                    dbo.GetStoreCountByCompany(@CompanyId) AS TotalStores,
                    dbo.GetProductCountByCompany(@CompanyId) AS TotalProducts,
                    dbo.GetTotalInventoryValueByCompany(@CompanyId) AS TotalInventoryValue,
                    dbo.GetLowStockProductsCount(@CompanyId) AS LowStockProductsCount
                FROM Companies c
                WHERE c.Id = @CompanyId;
            END;
        ");
    }

    public override void Down()
    {
        Execute.Sql("DROP PROCEDURE IF EXISTS dbo.InsertProduct");
        Execute.Sql("DROP PROCEDURE IF EXISTS dbo.InsertStore");
        Execute.Sql("DROP PROCEDURE IF EXISTS dbo.InsertCompany");
        Execute.Sql("DROP PROCEDURE IF EXISTS dbo.UpdateProductStock");
        Execute.Sql("DROP PROCEDURE IF EXISTS dbo.GetCompanyDashboard");
    }
}

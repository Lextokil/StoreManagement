using FluentMigrator;

namespace StoreManagement.Database.Migrations;

[Migration(004)]
public class Migration004_CreateInsertProcedures : Migration
{
    public override void Up()
    {
        // Create sp_InsertCompany procedure
        Execute.Sql(@"
            CREATE PROCEDURE sp_InsertCompany
                @Name NVARCHAR(255),
                @Code INT,
                @IsActive BIT = 1,
                @NewId UNIQUEIDENTIFIER OUTPUT,
                @ErrorMessage NVARCHAR(500) OUTPUT
            AS
            BEGIN
                SET NOCOUNT ON;
                
                BEGIN TRY
                    -- Initialize output parameters
                    SET @NewId = NEWID();
                    SET @ErrorMessage = NULL;
                    
                    -- Validate required fields
                    IF @Name IS NULL OR LTRIM(RTRIM(@Name)) = ''
                    BEGIN
                        SET @ErrorMessage = 'Company name is required';
                        RETURN -1;
                    END
                    
                    IF @Code IS NULL
                    BEGIN
                        SET @ErrorMessage = 'Company code is required';
                        RETURN -1;
                    END
                    
                    -- Check if code already exists
                    IF EXISTS (SELECT 1 FROM companies WHERE code = @Code)
                    BEGIN
                        SET @ErrorMessage = 'Company code already exists';
                        RETURN -2;
                    END
                    
                    -- Insert company
                    INSERT INTO companies (id, name, code, is_active, created_at, updated_at)
                    VALUES (@NewId, LTRIM(RTRIM(@Name)), @Code, @IsActive, GETUTCDATE(), NULL);
                    
                    RETURN 0; -- Success
                    
                END TRY
                BEGIN CATCH
                    SET @ErrorMessage = ERROR_MESSAGE();
                    RETURN -3; -- General error
                END CATCH
            END
        ");

        // Create sp_InsertStore procedure
        Execute.Sql(@"
            CREATE PROCEDURE sp_InsertStore
                @Name NVARCHAR(255),
                @Code INT,
                @Address NVARCHAR(500) = NULL,
                @CompanyId UNIQUEIDENTIFIER,
                @IsActive BIT = 1,
                @NewId UNIQUEIDENTIFIER OUTPUT,
                @ErrorMessage NVARCHAR(500) OUTPUT
            AS
            BEGIN
                SET NOCOUNT ON;
                
                BEGIN TRY
                    -- Initialize output parameters
                    SET @NewId = NEWID();
                    SET @ErrorMessage = NULL;
                    
                    -- Validate required fields
                    IF @Name IS NULL OR LTRIM(RTRIM(@Name)) = ''
                    BEGIN
                        SET @ErrorMessage = 'Store name is required';
                        RETURN -1;
                    END
                    
                    IF @Code IS NULL
                    BEGIN
                        SET @ErrorMessage = 'Store code is required';
                        RETURN -1;
                    END
                    
                    IF @CompanyId IS NULL
                    BEGIN
                        SET @ErrorMessage = 'Company ID is required';
                        RETURN -1;
                    END
                    
                    -- Check if company exists
                    IF NOT EXISTS (SELECT 1 FROM companies WHERE id = @CompanyId)
                    BEGIN
                        SET @ErrorMessage = 'Company not found';
                        RETURN -3;
                    END
                    
                    -- Check if code already exists for this company
                    IF EXISTS (SELECT 1 FROM stores WHERE code = @Code AND company_id = @CompanyId)
                    BEGIN
                        SET @ErrorMessage = 'Store code already exists for this company';
                        RETURN -2;
                    END
                    
                    -- Insert store
                    INSERT INTO stores (id, name, code, address, company_id, is_active, created_at, updated_at)
                    VALUES (@NewId, LTRIM(RTRIM(@Name)), @Code, @Address, @CompanyId, @IsActive, GETUTCDATE(), NULL);
                    
                    RETURN 0; -- Success
                    
                END TRY
                BEGIN CATCH
                    SET @ErrorMessage = ERROR_MESSAGE();
                    RETURN -3; -- General error
                END CATCH
            END
        ");

        // Create sp_InsertProduct procedure
        Execute.Sql(@"
            CREATE PROCEDURE sp_InsertProduct
                @Name NVARCHAR(255),
                @Description NVARCHAR(1000) = NULL,
                @Price DECIMAL(18,2),
                @Code INT,
                @StoreId UNIQUEIDENTIFIER,
                @IsActive BIT = 1,
                @NewId UNIQUEIDENTIFIER OUTPUT,
                @ErrorMessage NVARCHAR(500) OUTPUT
            AS
            BEGIN
                SET NOCOUNT ON;
                
                BEGIN TRY
                    -- Initialize output parameters
                    SET @NewId = NEWID();
                    SET @ErrorMessage = NULL;
                    
                    -- Validate required fields
                    IF @Name IS NULL OR LTRIM(RTRIM(@Name)) = ''
                    BEGIN
                        SET @ErrorMessage = 'Product name is required';
                        RETURN -1;
                    END
                    
                    IF @Code IS NULL
                    BEGIN
                        SET @ErrorMessage = 'Product code is required';
                        RETURN -1;
                    END
                    
                    IF @Price IS NULL OR @Price < 0
                    BEGIN
                        SET @ErrorMessage = 'Product price must be greater than or equal to zero';
                        RETURN -1;
                    END
                    
                    IF @StoreId IS NULL
                    BEGIN
                        SET @ErrorMessage = 'Store ID is required';
                        RETURN -1;
                    END
                    
                    -- Check if store exists
                    IF NOT EXISTS (SELECT 1 FROM stores WHERE id = @StoreId)
                    BEGIN
                        SET @ErrorMessage = 'Store not found';
                        RETURN -3;
                    END
                    
                    -- Check if code already exists for this store
                    IF EXISTS (SELECT 1 FROM products WHERE code = @Code AND store_id = @StoreId)
                    BEGIN
                        SET @ErrorMessage = 'Product code already exists for this store';
                        RETURN -2;
                    END
                    
                    -- Insert product
                    INSERT INTO products (id, name, description, price, code, store_id, is_active, created_at, updated_at)
                    VALUES (@NewId, LTRIM(RTRIM(@Name)), @Description, @Price, @Code, @StoreId, @IsActive, GETUTCDATE(), NULL);
                    
                    RETURN 0; -- Success
                    
                END TRY
                BEGIN CATCH
                    SET @ErrorMessage = ERROR_MESSAGE();
                    RETURN -3; -- General error
                END CATCH
            END
        ");
    }

    public override void Down()
    {
        Execute.Sql("DROP PROCEDURE IF EXISTS sp_InsertCompany");
        Execute.Sql("DROP PROCEDURE IF EXISTS sp_InsertStore");
        Execute.Sql("DROP PROCEDURE IF EXISTS sp_InsertProduct");
    }
}

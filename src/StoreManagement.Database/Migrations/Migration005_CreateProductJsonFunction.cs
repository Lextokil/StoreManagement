using FluentMigrator;

namespace StoreManagement.Database.Migrations;

[Migration(005)]
public class Migration005_CreateProductJsonFunction : Migration
{
    public override void Up()
    {
        Execute.Sql(@"
            CREATE FUNCTION fn_GetProductsAsJson(@StoreId UNIQUEIDENTIFIER)
            RETURNS NVARCHAR(MAX)
            AS
            BEGIN
                DECLARE @JsonResult NVARCHAR(MAX);
                
                -- Verificar se a store existe
                IF NOT EXISTS (SELECT 1 FROM stores WHERE id = @StoreId)
                BEGIN
                    RETURN '{""error"": ""Store not found""}';
                END
                
                -- Gerar JSON com produtos ativos da store
                SELECT @JsonResult = (
                    SELECT 
                        id,
                        name,
                        description,
                        price,
                        code,
                        store_id,
                        is_active,
                        created_at,
                        updated_at
                    FROM products 
                    WHERE store_id = @StoreId AND is_active = 1
                    FOR JSON AUTO, ROOT('products')
                );
                
                -- Se não houver produtos, retornar array vazio
                IF @JsonResult IS NULL
                    SET @JsonResult = '{""products"": []}';
                
                RETURN @JsonResult;
            END
        ");
    }

    public override void Down()
    {
        Execute.Sql("DROP FUNCTION IF EXISTS fn_GetProductsAsJson");
    }
}

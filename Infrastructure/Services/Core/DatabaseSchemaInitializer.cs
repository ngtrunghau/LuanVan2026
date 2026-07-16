using badmintion.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace badmintion.Services.Core
{
    public static class DatabaseSchemaInitializer
    {
        public static async Task EnsureApplicationSchemaAsync(this WebApplication app)
        {
            await using var scope = app.Services.CreateAsyncScope();
            var context = scope.ServiceProvider.GetRequiredService<BadmintionNlContext>();

            var commands = new[]
            {
                """
                IF OBJECT_ID(N'dbo.shipping_detail', N'U') IS NOT NULL
                   AND COL_LENGTH('shipping_detail', 'note') IS NULL
                    ALTER TABLE shipping_detail ADD note NVARCHAR(500) NULL;
                """,
                """
                IF OBJECT_ID(N'dbo.shipping_detail', N'U') IS NOT NULL
                   AND COL_LENGTH('shipping_detail', 'changed_by') IS NULL
                    ALTER TABLE shipping_detail ADD changed_by NVARCHAR(100) NULL;
                """,
                """
                IF OBJECT_ID(N'dbo.shipping_detail', N'U') IS NOT NULL
                   AND COL_LENGTH('shipping_detail', 'changed_by_type') IS NULL
                    ALTER TABLE shipping_detail ADD changed_by_type NVARCHAR(30) NULL;
                """,
                """
                IF OBJECT_ID(N'dbo.orders', N'U') IS NOT NULL
                   AND COL_LENGTH('orders', 'promotion_id') IS NULL
                    ALTER TABLE orders ADD promotion_id INT NULL;
                """,
                """
                IF OBJECT_ID(N'dbo.orders', N'U') IS NOT NULL
                   AND COL_LENGTH('orders', 'discount_amount') IS NULL
                    ALTER TABLE orders ADD discount_amount DECIMAL(18,2) NULL;
                """,
                """
                IF OBJECT_ID(N'dbo.promotions', N'U') IS NOT NULL
                   AND COL_LENGTH('promotions', 'code') IS NULL
                    ALTER TABLE promotions ADD code NVARCHAR(50) NULL;
                """,
                """
                IF OBJECT_ID(N'dbo.promotions', N'U') IS NOT NULL
                   AND COL_LENGTH('promotions', 'start_date') IS NULL
                    ALTER TABLE promotions ADD start_date DATETIME NULL;
                """,
                """
                IF OBJECT_ID(N'dbo.promotions', N'U') IS NOT NULL
                   AND COL_LENGTH('promotions', 'end_date') IS NULL
                    ALTER TABLE promotions ADD end_date DATETIME NULL;
                """,
                """
                IF OBJECT_ID(N'dbo.promotions', N'U') IS NOT NULL
                   AND COL_LENGTH('promotions', 'max_usage') IS NULL
                    ALTER TABLE promotions ADD max_usage INT NULL;
                """,
                """
                IF OBJECT_ID(N'dbo.promotions', N'U') IS NOT NULL
                   AND COL_LENGTH('promotions', 'used_count') IS NULL
                    ALTER TABLE promotions ADD used_count INT NOT NULL CONSTRAINT DF_promotions_used_count DEFAULT 0;
                """,
                """
                IF OBJECT_ID(N'dbo.product_review', N'U') IS NULL
                BEGIN
                    CREATE TABLE dbo.product_review
                    (
                        id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_product_review PRIMARY KEY,
                        product_id INT NULL,
                        customer_id INT NULL,
                        order_id INT NULL,
                        total_star INT NULL,
                        comment NVARCHAR(255) NULL,
                        url_img NVARCHAR(255) NULL,
                        is_deleted BIT NULL CONSTRAINT DF_product_review_is_deleted DEFAULT 0,
                        date DATETIME NULL,
                        moderation_status INT NOT NULL CONSTRAINT DF_product_review_moderation_status DEFAULT 1,
                        moderation_reason NVARCHAR(500) NULL,
                        moderated_at DATETIME NULL,
                        moderated_by NVARCHAR(100) NULL
                    );

                    IF OBJECT_ID(N'dbo.products', N'U') IS NOT NULL
                        ALTER TABLE dbo.product_review
                        ADD CONSTRAINT FK_product_review_products
                        FOREIGN KEY (product_id) REFERENCES dbo.products(id);
                END
                """,
                """
                IF OBJECT_ID(N'dbo.product_review', N'U') IS NOT NULL
                   AND COL_LENGTH('product_review', 'customer_id') IS NULL
                    ALTER TABLE product_review ADD customer_id INT NULL;
                """,
                """
                IF OBJECT_ID(N'dbo.product_review', N'U') IS NOT NULL
                   AND COL_LENGTH('product_review', 'order_id') IS NULL
                    ALTER TABLE product_review ADD order_id INT NULL;
                """,
                """
                IF OBJECT_ID(N'dbo.product_review', N'U') IS NOT NULL
                   AND COL_LENGTH('product_review', 'moderation_status') IS NULL
                    ALTER TABLE product_review ADD moderation_status INT NOT NULL
                    CONSTRAINT DF_product_review_moderation_status DEFAULT 1;
                """,
                """
                IF OBJECT_ID(N'dbo.product_review', N'U') IS NOT NULL
                   AND COL_LENGTH('product_review', 'moderation_reason') IS NULL
                    ALTER TABLE product_review ADD moderation_reason NVARCHAR(500) NULL;
                """,
                """
                IF OBJECT_ID(N'dbo.product_review', N'U') IS NOT NULL
                   AND COL_LENGTH('product_review', 'moderated_at') IS NULL
                    ALTER TABLE product_review ADD moderated_at DATETIME NULL;
                """,
                """
                IF OBJECT_ID(N'dbo.product_review', N'U') IS NOT NULL
                   AND COL_LENGTH('product_review', 'moderated_by') IS NULL
                    ALTER TABLE product_review ADD moderated_by NVARCHAR(100) NULL;
                """,
            };

            foreach (var command in commands)
            {
                await context.Database.ExecuteSqlRawAsync(command);
            }

            await EnsureNullableDateTimeColumnAsync<Customer>(
                context,
                nameof(Customer.PasswordChangedAt));
            await EnsureNullableDateTimeColumnAsync<User>(
                context,
                nameof(User.PasswordChangedAt));
        }

        private static async Task EnsureNullableDateTimeColumnAsync<TEntity>(
            BadmintionNlContext context,
            string propertyName)
            where TEntity : class
        {
            var entityType = context.Model.FindEntityType(typeof(TEntity))
                ?? throw new InvalidOperationException($"Entity {typeof(TEntity).Name} is not mapped.");
            var property = entityType.FindProperty(propertyName)
                ?? throw new InvalidOperationException($"Property {propertyName} is not mapped.");

            var tableName = entityType.GetTableName()
                ?? throw new InvalidOperationException($"Entity {typeof(TEntity).Name} does not map to a table.");
            var schema = entityType.GetSchema() ?? "dbo";
            var storeObject = StoreObjectIdentifier.Table(tableName, schema);
            var columnName = property.GetColumnName(storeObject) ?? property.GetColumnName();

            var fullTableName = $"{schema}.{tableName}";
            var command = $"""
                IF OBJECT_ID(N'{EscapeSqlLiteral(fullTableName)}', N'U') IS NOT NULL
                   AND COL_LENGTH(N'{EscapeSqlLiteral(fullTableName)}', N'{EscapeSqlLiteral(columnName)}') IS NULL
                    ALTER TABLE {QuoteIdentifier(schema)}.{QuoteIdentifier(tableName)}
                    ADD {QuoteIdentifier(columnName)} DATETIME NULL;
                """;

            await context.Database.ExecuteSqlRawAsync(command);
        }

        private static string QuoteIdentifier(string identifier)
        {
            return $"[{identifier.Replace("]", "]]")}]";
        }

        private static string EscapeSqlLiteral(string value)
        {
            return value.Replace("'", "''");
        }
    }
}

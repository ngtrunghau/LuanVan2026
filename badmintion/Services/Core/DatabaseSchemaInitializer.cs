using badmintion.Models;
using Microsoft.EntityFrameworkCore;

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
                IF COL_LENGTH('shipping_detail', 'note') IS NULL
                    ALTER TABLE shipping_detail ADD note NVARCHAR(500) NULL;
                """,
                """
                IF COL_LENGTH('shipping_detail', 'changed_by') IS NULL
                    ALTER TABLE shipping_detail ADD changed_by NVARCHAR(100) NULL;
                """,
                """
                IF COL_LENGTH('shipping_detail', 'changed_by_type') IS NULL
                    ALTER TABLE shipping_detail ADD changed_by_type NVARCHAR(30) NULL;
                """,
                """
                IF COL_LENGTH('orders', 'promotion_id') IS NULL
                    ALTER TABLE orders ADD promotion_id INT NULL;
                """,
                """
                IF COL_LENGTH('orders', 'discount_amount') IS NULL
                    ALTER TABLE orders ADD discount_amount DECIMAL(18,2) NULL;
                """,
                """
                IF COL_LENGTH('promotions', 'code') IS NULL
                    ALTER TABLE promotions ADD code NVARCHAR(50) NULL;
                """,
                """
                IF COL_LENGTH('promotions', 'start_date') IS NULL
                    ALTER TABLE promotions ADD start_date DATETIME NULL;
                """,
                """
                IF COL_LENGTH('promotions', 'end_date') IS NULL
                    ALTER TABLE promotions ADD end_date DATETIME NULL;
                """,
                """
                IF COL_LENGTH('promotions', 'max_usage') IS NULL
                    ALTER TABLE promotions ADD max_usage INT NULL;
                """,
                """
                IF COL_LENGTH('promotions', 'used_count') IS NULL
                    ALTER TABLE promotions ADD used_count INT NOT NULL CONSTRAINT DF_promotions_used_count DEFAULT 0;
                """,
                """
                IF COL_LENGTH('product_review', 'customer_id') IS NULL
                    ALTER TABLE product_review ADD customer_id INT NULL;
                """,
                """
                IF COL_LENGTH('product_review', 'order_id') IS NULL
                    ALTER TABLE product_review ADD order_id INT NULL;
                """,
                """
                IF COL_LENGTH('product_review', 'moderation_status') IS NULL
                    ALTER TABLE product_review ADD moderation_status INT NOT NULL
                    CONSTRAINT DF_product_review_moderation_status DEFAULT 1;
                """,
                """
                IF COL_LENGTH('product_review', 'moderation_reason') IS NULL
                    ALTER TABLE product_review ADD moderation_reason NVARCHAR(500) NULL;
                """,
                """
                IF COL_LENGTH('product_review', 'moderated_at') IS NULL
                    ALTER TABLE product_review ADD moderated_at DATETIME NULL;
                """,
                """
                IF COL_LENGTH('product_review', 'moderated_by') IS NULL
                    ALTER TABLE product_review ADD moderated_by NVARCHAR(100) NULL;
                """,
                """
                IF COL_LENGTH('Customer', 'password_changed_at') IS NULL
                    ALTER TABLE Customer ADD password_changed_at DATETIME NULL;
                """,
                """
                IF COL_LENGTH('Users', 'password_changed_at') IS NULL
                    ALTER TABLE Users ADD password_changed_at DATETIME NULL;
                """
            };

            foreach (var command in commands)
            {
                await context.Database.ExecuteSqlRawAsync(command);
            }
        }
    }
}

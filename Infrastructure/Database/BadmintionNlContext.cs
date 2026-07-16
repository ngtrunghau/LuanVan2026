using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace badmintion.Models;

public partial class BadmintionNlContext : DbContext
{
    public BadmintionNlContext()
    {
    }

    public BadmintionNlContext(DbContextOptions<BadmintionNlContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AddressCustomer> AddressCustomers { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<ControllerManage> ControllerManages { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<District> Districts { get; set; }

    public virtual DbSet<FunctionManage> FunctionManages { get; set; }

    public virtual DbSet<HistoryImport> HistoryImports { get; set; }

    public virtual DbSet<InventoryAlert> InventoryAlerts { get; set; }

    public virtual DbSet<Login> Logins { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderItem> OrderItems { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductReview> ProductReviews { get; set; }

    public virtual DbSet<Promotion> Promotions { get; set; }

    public virtual DbSet<Province> Provinces { get; set; }

    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

    public virtual DbSet<SalesReport> SalesReports { get; set; }

    public virtual DbSet<ShippingDetail> ShippingDetails { get; set; }

    public virtual DbSet<Town> Towns { get; set; }

    public virtual DbSet<UnitRole> UnitRoles { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<WareHouse> WareHouses { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AddressCustomer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__address___3213E83FB3DF00D6");

            entity.ToTable("address_customer");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Address)
                .HasMaxLength(100)
                .HasColumnName("address");
            entity.Property(e => e.CustomerId).HasColumnName("customer_id");
            entity.Property(e => e.DistrictId).HasColumnName("district_id");
            entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");
            entity.Property(e => e.ProvinceId).HasColumnName("province_id");
            entity.Property(e => e.TownId).HasColumnName("town_id");

            entity.HasOne(d => d.Customer).WithMany(p => p.AddressCustomers)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK__address_c__custo__5629CD9C");

            entity.HasOne(d => d.District).WithMany(p => p.AddressCustomers)
                .HasForeignKey(d => d.DistrictId)
                .HasConstraintName("FK__address_c__distr__5441852A");

            entity.HasOne(d => d.Province).WithMany(p => p.AddressCustomers)
                .HasForeignKey(d => d.ProvinceId)
                .HasConstraintName("FK__address_c__provi__534D60F1");

            entity.HasOne(d => d.Town).WithMany(p => p.AddressCustomers)
                .HasForeignKey(d => d.TownId)
                .HasConstraintName("FK__address_c__town___5535A963");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__categori__3213E83FA7C3D92E");

            entity.ToTable("categories");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.Sort).HasColumnName("sort");
        });

        modelBuilder.Entity<ControllerManage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__controll__3213E83F51E84ADA");

            entity.ToTable("controller_manage");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");
            entity.Property(e => e.Key)
                .HasMaxLength(255)
                .HasColumnName("key");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Customer__3213E83F7BAFAD4A");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.FullName)
                .HasMaxLength(255)
                .HasColumnName("full_name");
            entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("password");
            entity.Property(e => e.PasswordChangedAt)
                .HasColumnType("datetime")
                .HasColumnName("password_changed_at");
            entity.Property(e => e.Phone)
                .HasMaxLength(10)
                .HasColumnName("phone");
            entity.Property(e => e.UserName)
                .HasMaxLength(255)
                .HasColumnName("user_name");
        });

        modelBuilder.Entity<District>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__district__3213E83FEC8796C2");

            entity.ToTable("district");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.ProvinceId).HasColumnName("province_id");

            entity.HasOne(d => d.Province).WithMany(p => p.Districts)
                .HasForeignKey(d => d.ProvinceId)
                .HasConstraintName("FK__district__provin__4BAC3F29");
        });

        modelBuilder.Entity<FunctionManage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__function__3213E83F285F8E4B");

            entity.ToTable("function_manage");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ControllerId).HasColumnName("controller_id");
            entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");
            entity.Property(e => e.Key)
                .HasMaxLength(255)
                .HasColumnName("key");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Router)
                .HasMaxLength(255)
                .HasColumnName("router");
            entity.Property(e => e.UnitRoleId).HasColumnName("unit_role_id");

            entity.HasOne(d => d.Controller).WithMany(p => p.FunctionManages)
                .HasForeignKey(d => d.ControllerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__function___contr__19DFD96B");

            entity.HasOne(d => d.UnitRole).WithMany(p => p.FunctionManages)
                .HasForeignKey(d => d.UnitRoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__function___unit___1AD3FDA4");
        });

        modelBuilder.Entity<HistoryImport>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__history___3213E83F53A77E17");

            entity.ToTable("history_import");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.DateImport)
                .HasColumnType("datetime")
                .HasColumnName("date_import");
            entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.QuantityImport).HasColumnName("quantity_import");
            entity.Property(e => e.WareHouseId).HasColumnName("ware_house_id");

            entity.HasOne(d => d.Product).WithMany(p => p.HistoryImports)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK__history_i__produ__2BFE89A6");

            entity.HasOne(d => d.WareHouse).WithMany(p => p.HistoryImports)
                .HasForeignKey(d => d.WareHouseId)
                .HasConstraintName("FK__history_i__ware___2CF2ADDF");
        });

        modelBuilder.Entity<InventoryAlert>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__inventor__3213E83FE00E7D6B");

            entity.ToTable("inventory_alerts");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AlertThreshold).HasColumnName("alert_threshold");
            entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");
            entity.Property(e => e.ProductsId).HasColumnName("products_id");

            entity.HasOne(d => d.Products).WithMany(p => p.InventoryAlerts)
                .HasForeignKey(d => d.ProductsId)
                .HasConstraintName("FK__inventory__produ__5DCAEF64");
        });

        modelBuilder.Entity<Login>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__login__3213E83F459737A0");

            entity.ToTable("login");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AccessToken)
                .HasColumnType("text")
                .HasColumnName("access_token");
            entity.Property(e => e.Date)
                .HasColumnType("datetime")
                .HasColumnName("date");
            entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");
            entity.Property(e => e.RefreshToken)
                .HasColumnType("text")
                .HasColumnName("refresh_token");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Logins)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__login__user_id__3C34F16F");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__orders__3213E83F6E0C86F6");

            entity.ToTable("orders");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AddressId).HasColumnName("address_id");
            entity.Property(e => e.CustomerId).HasColumnName("customer_id");
            entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");
            entity.Property(e => e.OrderDate)
                .HasColumnType("datetime")
                .HasColumnName("order_date");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.PromotionId).HasColumnName("promotion_id");
            entity.Property(e => e.DiscountAmount)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("discount_amount");
            entity.Property(e => e.TotalAmount)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("total_amount");
            entity.Property(e => e.TxnRef)
                .HasMaxLength(255)
                .HasColumnName("txn_ref");

            entity.HasOne(d => d.Address).WithMany(p => p.Orders)
                .HasForeignKey(d => d.AddressId)
                .HasConstraintName("FK__orders__address___4D5F7D71");

            entity.HasOne(d => d.Customer).WithMany(p => p.Orders)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK__orders__customer__68487DD7");
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__order_it__3213E83F652EE745");

            entity.ToTable("order_items");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");
            entity.Property(e => e.Orders).HasColumnName("orders");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("price");
            entity.Property(e => e.ProductsId).HasColumnName("products_id");
            entity.Property(e => e.Quantity).HasColumnName("quantity");

            entity.HasOne(d => d.OrdersNavigation).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.Orders)
                .HasConstraintName("FK__order_ite__order__73BA3083");

            entity.HasOne(d => d.Products).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.ProductsId)
                .HasConstraintName("FK__order_ite__produ__72C60C4A");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__payment__3213E83F0A6032A9");

            entity.ToTable("payment");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BillId)
                .HasMaxLength(255)
                .HasColumnName("bill_id");
            entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");
            entity.Property(e => e.OrdersId).HasColumnName("orders_id");
            entity.Property(e => e.PaymentMethod)
                .HasMaxLength(255)
                .HasColumnName("payment_method");
            entity.Property(e => e.PaymentStatus)
                .HasMaxLength(255)
                .HasColumnName("payment_status");

            entity.HasOne(d => d.Orders).WithMany(p => p.Payments)
                .HasForeignKey(d => d.OrdersId)
                .HasConstraintName("FK__payment__orders__6EF57B66");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__products__3213E83F78AF4B19");

            entity.ToTable("products");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CategoriesId).HasColumnName("categories_id");
            entity.Property(e => e.Color)
                .HasMaxLength(255)
                .HasColumnName("color");
            entity.Property(e => e.Descriptions).HasColumnName("descriptions");
            entity.Property(e => e.ImageUrl)
                .HasColumnType("text")
                .HasColumnName("image_url");
            entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("price");
            entity.Property(e => e.StockQuantity).HasColumnName("stock_quantity");

            entity.HasOne(d => d.Categories).WithMany(p => p.Products)
                .HasForeignKey(d => d.CategoriesId)
                .HasConstraintName("FK__products__catego__5AEE82B9");
        });

        modelBuilder.Entity<ProductReview>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__product___3213E83FA5E0B0AD");

            entity.ToTable("product_review");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Comment)
                .HasMaxLength(255)
                .HasColumnName("comment");
            entity.Property(e => e.Date)
                .HasColumnType("datetime")
                .HasColumnName("date");
            entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.CustomerId).HasColumnName("customer_id");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.ModerationStatus).HasColumnName("moderation_status");
            entity.Property(e => e.ModerationReason)
                .HasMaxLength(500)
                .HasColumnName("moderation_reason");
            entity.Property(e => e.ModeratedAt)
                .HasColumnType("datetime")
                .HasColumnName("moderated_at");
            entity.Property(e => e.ModeratedBy)
                .HasMaxLength(100)
                .HasColumnName("moderated_by");
            entity.Property(e => e.TotalStar).HasColumnName("total_star");
            entity.Property(e => e.UrlImg)
                .HasMaxLength(255)
                .HasColumnName("url_img");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductReviews)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK__product_r__produ__625A9A57");
        });

        modelBuilder.Entity<Promotion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__promotio__3213E83FED99D4D0");

            entity.ToTable("promotions");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Descriptions)
                .HasColumnType("text")
                .HasColumnName("descriptions");
            entity.Property(e => e.DiscountType)
                .HasMaxLength(255)
                .HasColumnName("discount_type");
            entity.Property(e => e.DiscountValue)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("discount_value");
            entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");
            entity.Property(e => e.MinOrderValue)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("min_order_value");
            entity.Property(e => e.Code)
                .HasMaxLength(50)
                .HasColumnName("code");
            entity.Property(e => e.StartDate)
                .HasColumnType("datetime")
                .HasColumnName("start_date");
            entity.Property(e => e.EndDate)
                .HasColumnType("datetime")
                .HasColumnName("end_date");
            entity.Property(e => e.MaxUsage).HasColumnName("max_usage");
            entity.Property(e => e.UsedCount).HasColumnName("used_count");
        });

        modelBuilder.Entity<Province>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__province__3213E83F5D1B94C0");

            entity.ToTable("province");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__refresh___3213E83F0A6498DE");

            entity.ToTable("refresh_token");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AccessToken)
                .HasColumnType("text")
                .HasColumnName("access_token");
            entity.Property(e => e.CreationDatetoken)
                .HasColumnType("datetime")
                .HasColumnName("creation_datetoken");
            entity.Property(e => e.ExpiryDatetoken)
                .HasColumnType("datetime")
                .HasColumnName("expiry_datetoken");
            entity.Property(e => e.Expirydaterefreshtoken).HasColumnName("expirydaterefreshtoken");
            entity.Property(e => e.Invalidated).HasColumnName("invalidated");
            entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");
            entity.Property(e => e.JwtId)
                .HasMaxLength(255)
                .HasColumnName("jwt_id");
            entity.Property(e => e.RefreshToken1)
                .HasColumnType("text")
                .HasColumnName("refresh_token");
            entity.Property(e => e.UnitRoleId).HasColumnName("unit_role_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.UnitRole).WithMany(p => p.RefreshTokens)
                .HasForeignKey(d => d.UnitRoleId)
                .HasConstraintName("FK__refresh_t__unit___3F115E1A");

            entity.HasOne(d => d.User).WithMany(p => p.RefreshTokens)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__refresh_t__user___40058253");
        });

        modelBuilder.Entity<SalesReport>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__sales_re__3213E83F17393908");

            entity.ToTable("sales_reports");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");
            entity.Property(e => e.Period)
                .HasMaxLength(255)
                .HasColumnName("period");
            entity.Property(e => e.ReportDate)
                .HasColumnType("datetime")
                .HasColumnName("report_date");
            entity.Property(e => e.TotalSales)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("total_sales");
        });

        modelBuilder.Entity<ShippingDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__shipping__3213E83FAD28DBB6");

            entity.ToTable("shipping_detail");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.DateShip)
                .HasColumnType("datetime")
                .HasColumnName("date_ship");
            entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");
            entity.Property(e => e.OrdersId).HasColumnName("orders_id");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.Note)
                .HasMaxLength(500)
                .HasColumnName("note");
            entity.Property(e => e.ChangedBy)
                .HasMaxLength(100)
                .HasColumnName("changed_by");
            entity.Property(e => e.ChangedByType)
                .HasMaxLength(30)
                .HasColumnName("changed_by_type");

            entity.HasOne(d => d.Orders).WithMany(p => p.ShippingDetails)
                .HasForeignKey(d => d.OrdersId)
                .HasConstraintName("FK__shipping___order__02FC7413");
        });

        modelBuilder.Entity<Town>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__town__3213E83F666ED200");

            entity.ToTable("town");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.DistrictId).HasColumnName("district_id");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");

            entity.HasOne(d => d.District).WithMany(p => p.Towns)
                .HasForeignKey(d => d.DistrictId)
                .HasConstraintName("FK__town__district_i__4E88ABD4");
        });

        modelBuilder.Entity<UnitRole>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__unit_rol__3213E83F0D5CB664");

            entity.ToTable("unit_role");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3213E83F118E33FD");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("password");
            entity.Property(e => e.PasswordChangedAt)
                .HasColumnType("datetime")
                .HasColumnName("password_changed_at");
            entity.Property(e => e.UnitRoleId).HasColumnName("unit_role_id");
            entity.Property(e => e.UserName)
                .HasMaxLength(255)
                .HasColumnName("user_name");

            entity.HasOne(d => d.UnitRole).WithMany(p => p.Users)
                .HasForeignKey(d => d.UnitRoleId)
                .HasConstraintName("FK__Users__unit_role__1DB06A4F");
        });

        modelBuilder.Entity<WareHouse>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ware_hou__3213E83F9399753A");

            entity.ToTable("ware_house");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.QuantityImport).HasColumnName("quantity_import");
            entity.Property(e => e.RemainQuantity).HasColumnName("remain_quantity");

            entity.HasOne(d => d.Product).WithMany(p => p.WareHouses)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK__ware_hous__produ__29221CFB");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce_Product.Models;

public partial class EcommerceshopContext : DbContext
{
    public EcommerceshopContext()
    {
    }

    public EcommerceshopContext(DbContextOptions<EcommerceshopContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AspnetRole> AspNetRoles { get; set; }

    public virtual DbSet<AspNetRoleClaim> AspNetRoleClaims { get; set; }

    public virtual DbSet<AspnetUser> AspNetUsers { get; set; }

    public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }

    public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }

    public virtual DbSet<AspNetUserToken> AspNetUserTokens { get; set; }

    public virtual DbSet<Banner> Banners { get; set; }

    public virtual DbSet<Brand> Brands { get; set; }

    public virtual DbSet<Cart> Carts { get; set; }

    public virtual DbSet<CartDetail> CartDetails { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<CategoryBrandDetails> CategoryBrandDetails { get; set; }

    public virtual DbSet<Color> Colors { get; set; }

    public virtual DbSet<HistoryStore> HistoryStores { get; set; }

    public virtual DbSet<Mirror> Mirrors { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderDetail> OrderDetails { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductImage> ProductImages { get; set; }

    public virtual DbSet<Setting> Settings { get; set; }

    public virtual DbSet<Size> Sizes { get; set; }

    public virtual DbSet<StaticFiles> StaticFiles { get; set; }

    public virtual DbSet<SubCategories> SubCategories { get; set; }

    public virtual DbSet<Variant> Variants { get; set; }

    public virtual DbSet<Version> Versions { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=14.225.212.107;Port=5433;Database=ecommerceshop;Username=postgres;Password=miyuki123");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AspnetRole>(entity =>
        {
            entity.HasIndex(e => e.NormalizedName, "RoleNameIndex").IsUnique();

            entity.Property(e => e.Name).HasMaxLength(256);
            entity.Property(e => e.NormalizedName).HasMaxLength(256);
        });

        modelBuilder.Entity<AspNetRoleClaim>(entity =>
        {
            entity.HasIndex(e => e.RoleId, "IX_AspNetRoleClaims_RoleId");

            entity.HasOne(d => d.Role).WithMany(p => p.AspNetRoleClaims).HasForeignKey(d => d.RoleId);
        });

        modelBuilder.Entity<AspnetUser>(entity =>
        {
            entity.HasIndex(e => e.NormalizedEmail, "EmailIndex");

            entity.HasIndex(e => e.NormalizedUserName, "UserNameIndex").IsUnique();

            entity.Property(e => e.Address1).HasColumnType("character varying");
            entity.Property(e => e.Address2).HasColumnType("character varying");
            entity.Property(e => e.Avatar).HasColumnType("character varying");
            entity.Property(e => e.CreatedDate)
                .HasColumnType("character varying")
                .HasColumnName("Created_Date");
            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.Gender).HasColumnType("character varying");
            entity.Property(e => e.NormalizedEmail).HasMaxLength(256);
            entity.Property(e => e.NormalizedUserName).HasMaxLength(256);
            entity.Property(e => e.Seq).HasDefaultValueSql("nextval('user_number_seq'::regclass)");
            entity.Property(e => e.UserName).HasMaxLength(256);

            entity.HasMany(d => d.Roles).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "AspNetUserRole",
                    r => r.HasOne<AspnetRole>().WithMany().HasForeignKey("RoleId"),
                    l => l.HasOne<AspnetUser>().WithMany().HasForeignKey("UserId"),
                    j =>
                    {
                        j.HasKey("UserId", "RoleId");
                        j.ToTable("AspNetUserRoles");
                        j.HasIndex(new[] { "RoleId" }, "IX_AspNetUserRoles_RoleId");
                    });
        });

        modelBuilder.Entity<AspNetUserClaim>(entity =>
        {
            entity.HasIndex(e => e.UserId, "IX_AspNetUserClaims_UserId");

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserClaims).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<AspNetUserLogin>(entity =>
        {
            entity.HasKey(e => new { e.LoginProvider, e.ProviderKey });

            entity.HasIndex(e => e.UserId, "IX_AspNetUserLogins_UserId");

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserLogins).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<AspNetUserToken>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name });

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserTokens).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<Banner>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("banner_pk");

            entity.ToTable("Banner");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Bannername)
                .HasColumnType("character varying")
                .HasColumnName("bannername");
            entity.Property(e => e.Createddate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createddate");
            entity.Property(e => e.Image)
                .HasColumnType("character varying")
                .HasColumnName("image");
            entity.Property(e => e.Updateddate)
                .HasColumnType("character varying")
                .HasColumnName("updateddate");
        });

        modelBuilder.Entity<Brand>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("brand_pk");

            entity.ToTable("Brand");

            entity.Property(e => e.Id).HasDefaultValueSql("nextval('\"Brand_id_seq\"'::regclass)");
            entity.Property(e => e.BrandName).HasColumnType("character varying");
            entity.Property(e => e.CreatedDate)
                .HasColumnType("character varying")
                .HasColumnName("Created_Date");
            entity.Property(e => e.UpdatedDate)
                .HasColumnType("character varying")
                .HasColumnName("Updated_Date");
        });

        modelBuilder.Entity<Cart>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("cart_pk");

            entity.ToTable("Cart");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Createddate)
                .HasColumnType("character varying")
                .HasColumnName("createddate");
            entity.Property(e => e.Updateddate)
                .HasColumnType("character varying")
                .HasColumnName("updateddate");
            entity.Property(e => e.Userid)
                .HasColumnType("character varying")
                .HasColumnName("userid");

            entity.HasOne(d => d.User).WithMany(p => p.Carts)
                .HasForeignKey(d => d.Userid)
                .HasConstraintName("cart_user_fk");
        });

        modelBuilder.Entity<CartDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("cart_detail_pk");

            entity.ToTable("CartDetail");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Cartid).HasColumnName("cartid");
            entity.Property(e => e.Discount).HasColumnName("discount");
            entity.Property(e => e.Price).HasColumnName("price");
            entity.Property(e => e.Productid).HasColumnName("productid");
            entity.Property(e => e.Quantity).HasColumnName("quantity");

            entity.HasOne(d => d.Cart).WithMany(p => p.CartDetails)
                .HasForeignKey(d => d.Cartid)
                .HasConstraintName("cartdetail_cart_fk");

            entity.HasOne(d => d.Product).WithMany(p => p.CartDetails)
                .HasForeignKey(d => d.Productid)
                .HasConstraintName("cart_product_fk");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("category_pk");

            entity.ToTable("Category");

            entity.Property(e => e.Id).HasDefaultValueSql("nextval('category_id_seq'::regclass)");
            entity.Property(e => e.Avatar).HasColumnType("character varying");
            entity.Property(e => e.CategoryName).HasColumnType("character varying");
            entity.Property(e => e.CreatedDate)
                .HasColumnType("character varying")
                .HasColumnName("Created_Date");
            entity.Property(e => e.UpdatedDate)
                .HasColumnType("character varying")
                .HasColumnName("Updated_Date");
        });

        modelBuilder.Entity<CategoryBrandDetails>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("categorybranddetail_pk");

            entity.ToTable("CategoryBrandDetails");

            entity.Property(e => e.Id).HasDefaultValueSql("nextval('\"CategoryBrandDetail_id_seq\"'::regclass)");

            entity.HasOne(d => d.Brand).WithMany(p => p.CategoryBrandDetails)
                .HasForeignKey(d => d.BrandId)
                .HasConstraintName("brand_fk");

            entity.HasOne(d => d.Category).WithMany(p => p.CategoryBrandDetails)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("category_fk");
        });

        modelBuilder.Entity<Color>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("color_pk");

            entity.ToTable("color");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Colorname)
                .HasColumnType("character varying")
                .HasColumnName("colorname");
        });

        modelBuilder.Entity<HistoryStore>(entity =>
        {
            entity.HasKey(e => new { e.TableName, e.PkDateDest }).HasName("primary");

            entity.ToTable("history_store");

            entity.HasIndex(e => new { e.TableName, e.PkDateSrc }, "history_store_ix");

            entity.Property(e => e.TableName)
                .HasMaxLength(50)
                .HasColumnName("table_name");
            entity.Property(e => e.PkDateDest)
                .HasMaxLength(400)
                .HasColumnName("pk_date_dest");
            entity.Property(e => e.PkDateSrc)
                .HasMaxLength(400)
                .HasColumnName("pk_date_src");
            entity.Property(e => e.RecordState).HasColumnName("record_state");
            entity.Property(e => e.Timemark)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("timemark");
        });

        modelBuilder.Entity<Mirror>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("mirror_pk");

            entity.ToTable("Mirror");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Mirrorname)
                .HasColumnType("character varying")
                .HasColumnName("mirrorname");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("order_pk");

            entity.ToTable("Order");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Createddate)
                .HasColumnType("character varying")
                .HasColumnName("createddate");
            entity.Property(e => e.Paymentid).HasColumnName("paymentid");
            entity.Property(e => e.Shippingaddress)
                .HasColumnType("character varying")
                .HasColumnName("shippingaddress");
            entity.Property(e => e.Status)
                .HasColumnType("character varying")
                .HasColumnName("status");
            entity.Property(e => e.Total).HasColumnName("total");
            entity.Property(e => e.Userid)
                .HasColumnType("character varying")
                .HasColumnName("userid");

            entity.HasOne(d => d.Payment).WithMany(p => p.Orders)
                .HasForeignKey(d => d.Paymentid)
                .HasConstraintName("order_payment_fk");

            entity.HasOne(d => d.User).WithMany(p => p.Orders)
                .HasForeignKey(d => d.Userid)
                .HasConstraintName("order_user_fk");
        });

        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("order_detail_pk");

            entity.ToTable("OrderDetail");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Discount).HasColumnName("discount");
            entity.Property(e => e.Orderid).HasColumnName("orderid");
            entity.Property(e => e.Price).HasColumnName("price");
            entity.Property(e => e.Productid).HasColumnName("productid");
            entity.Property(e => e.Quantity).HasColumnName("quantity");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.Orderid)
                .HasConstraintName("orderdetail_order_fk");

            entity.HasOne(d => d.Product).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.Productid)
                .HasConstraintName("orderdetail_product_fk");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("payment_pk");

            entity.ToTable("Payment");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Createddate)
                .HasColumnType("character varying")
                .HasColumnName("createddate");
            entity.Property(e => e.Paymentname)
                .HasColumnType("character varying")
                .HasColumnName("paymentname");
            entity.Property(e => e.Updateddate)
                .HasColumnType("character varying")
                .HasColumnName("updateddate");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("product_pk");

            entity.ToTable("Product");

            entity.Property(e => e.Id).HasDefaultValueSql("nextval('\"Product_id_seq\"'::regclass)");
            entity.Property(e => e.Backavatar)
                .HasColumnType("character varying")
                .HasColumnName("backavatar");
            entity.Property(e => e.CreatedDate).HasColumnType("character varying");
            entity.Property(e => e.Description).HasColumnType("character varying");
            entity.Property(e => e.DiscountDescription).HasColumnType("character varying");
            entity.Property(e => e.Frontavatar)
                .HasColumnType("character varying")
                .HasColumnName("frontavatar");
            entity.Property(e => e.InboxDescription).HasColumnType("character varying");
            entity.Property(e => e.Price).HasColumnType("character varying");
            entity.Property(e => e.ProductName).HasColumnType("character varying");
            entity.Property(e => e.Status).HasColumnType("character varying");
            entity.Property(e => e.UpdatedDate).HasColumnType("character varying");

            entity.HasOne(d => d.Brand).WithMany(p => p.Products)
                .HasForeignKey(d => d.BrandId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("product_brand_fk");

            entity.HasOne(d => d.Category).WithMany(p => p.Products)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("product_cat_fk");

            entity.HasOne(d => d.SubCat).WithMany(p => p.Products)
                .HasForeignKey(d => d.SubCatId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("product_sub_cat_fk");
        });

        modelBuilder.Entity<ProductImage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("productimage_pk");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Avatar)
                .HasColumnType("character varying")
                .HasColumnName("avatar");
            entity.Property(e => e.Productid).HasColumnName("productid");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductImages)
                .HasForeignKey(d => d.Productid)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("product_img_fk");
        });

        modelBuilder.Entity<Setting>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("setting_pk");

            entity.ToTable("Setting");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.App)
                .HasColumnType("character varying")
                .HasColumnName("app");
            entity.Property(e => e.Createddate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("character varying")
                .HasColumnName("createddate");
            entity.Property(e => e.Settingname)
                .HasColumnType("character varying")
                .HasColumnName("settingname");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.Updateddate)
                .HasColumnType("character varying")
                .HasColumnName("updateddate");
        });

        modelBuilder.Entity<Size>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("size_pk");

            entity.ToTable("size");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Sizename)
                .HasColumnType("character varying")
                .HasColumnName("sizename");
        });

        modelBuilder.Entity<StaticFiles>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("staticfile_pk");

            entity.ToTable("StaticFiles");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Content)
                .HasColumnType("character varying")
                .HasColumnName("content");
            entity.Property(e => e.Createddate)
                .HasColumnType("character varying")
                .HasColumnName("createddate");
            entity.Property(e => e.Filename)
                .HasColumnType("character varying")
                .HasColumnName("filename");
            entity.Property(e => e.Updateddate)
                .HasColumnType("character varying")
                .HasColumnName("updateddate");
        });

        modelBuilder.Entity<SubCategories>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("subcategory_pk");

            entity.ToTable("SubCategories");

            entity.Property(e => e.Id).HasDefaultValueSql("nextval('subcategory_id_seq'::regclass)");
            entity.Property(e => e.CreatedDate)
                .HasColumnType("character varying")
                .HasColumnName("Created_Date");
            entity.Property(e => e.SubCategoryName).HasColumnType("character varying");
            entity.Property(e => e.UpdatedDate)
                .HasColumnType("character varying")
                .HasColumnName("Updated_Date");

            entity.HasOne(d => d.Category).WithMany(p => p.SubCategories)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("sub_cat_fk");
        });

        modelBuilder.Entity<Variant>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("variant_pk");

            entity.ToTable("variant");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Colorid).HasColumnName("colorid");
            entity.Property(e => e.Mirrorid).HasColumnName("mirrorid");
            entity.Property(e => e.Productid).HasColumnName("productid");
            entity.Property(e => e.Sizeid).HasColumnName("sizeid");
            entity.Property(e => e.Versionid).HasColumnName("versionid");
            entity.Property(e => e.Weight).HasColumnName("weight");

            entity.HasOne(d => d.Color).WithMany(p => p.Variants)
                .HasForeignKey(d => d.Colorid)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("color_fk");

            entity.HasOne(d => d.Mirror).WithMany(p => p.Variants)
                .HasForeignKey(d => d.Mirrorid)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("mirror_fk");

            entity.HasOne(d => d.Product).WithMany(p => p.Variants)
                .HasForeignKey(d => d.Productid)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("product_fk");

            entity.HasOne(d => d.Size).WithMany(p => p.Variants)
                .HasForeignKey(d => d.Sizeid)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("size_fk");

            entity.HasOne(d => d.Version).WithMany(p => p.Variants)
                .HasForeignKey(d => d.Versionid)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("version_fk");
        });

        modelBuilder.Entity<Version>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("version_pk");

            entity.ToTable("Version");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Versionname)
                .HasColumnType("character varying")
                .HasColumnName("versionname");
        });
        modelBuilder.HasSequence("user_number_seq");

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
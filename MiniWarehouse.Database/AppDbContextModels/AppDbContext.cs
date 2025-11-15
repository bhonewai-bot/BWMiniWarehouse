using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace MiniWarehouse.Database.AppDbContextModels;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<TblInventoryTransaction> TblInventoryTransactions { get; set; }

    public virtual DbSet<TblItem> TblItems { get; set; }

    public virtual DbSet<TblStock> TblStocks { get; set; }
    
    public virtual DbSet<TblSupplier> TblSuppliers { get; set; }
    
    public virtual DbSet<TblCustomer> TblCustomers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            string connectionString = "Server=.;Database=MiniWarehouse;User ID=sa;Password=sasa@123;TrustServerCertificate=True;";
            optionsBuilder.UseSqlServer(connectionString);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TblInventoryTransaction>(entity =>
        {
            entity.HasKey(e => e.TransactionId).HasName("PK__Tbl_Inve__55433A6B90ADCED2");

            entity.ToTable("Tbl_InventoryTransactions");

            entity.Property(e => e.TransactionDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Type)
                .HasConversion<string>()
                .HasMaxLength(10)       
                .IsRequired();

            entity.HasOne(d => d.Item).WithMany(p => p.TblInventoryTransactions)
                .HasForeignKey(d => d.ItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Tbl_Inven__ItemI__3D5E1FD2");

            entity.HasOne(d => d.Supplier).WithMany(p => p.TblInventoryTransactions)
                .HasForeignKey(d => d.SupplierId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Customer).WithMany(p => p.TblInventoryTransactions)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<TblItem>(entity =>
        {
            entity.HasKey(e => e.ItemId).HasName("PK__Tbl_Item__727E838BEA6AE3A4");

            entity.ToTable("Tbl_Items");

            entity.HasIndex(e => e.Sku, "UQ__Tbl_Item__CA1ECF0D680CB04B").IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ItemName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Sku)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("SKU");
        });

        modelBuilder.Entity<TblStock>(entity =>
        {
            entity.HasKey(e => e.StockId).HasName("PK__Tbl_Stoc__2C83A9C21FE2FFBF");

            entity.ToTable("Tbl_Stocks");

            entity.HasOne(d => d.Item).WithMany(p => p.TblStocks)
                .HasForeignKey(d => d.ItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Tbl_Stock__ItemI__412EB0B6");
        });

        modelBuilder.Entity<TblSupplier>(entity =>
        {
            entity.HasKey(e => e.SupplierId);

            entity.ToTable("Tbl_Suppliers");

            entity.Property(e => e.SupplierName)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<TblCustomer>(entity =>
        {
            entity.HasKey(e => e.CustomerId);
            
            entity.ToTable("Tbl_Customers");

            entity.Property(e => e.CustomerName)
                .HasMaxLength(50)
                .IsUnicode(false);
        });
            
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
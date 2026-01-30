// File: Data/AppDbContext.cs
using Microsoft.EntityFrameworkCore;
using SaleManagementAPI.Models;
using SaleManagementAPI.Models;

namespace SaleManagementAPI.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ===== Users =====
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(x => x.Username).IsUnique(); // Username unique
            entity.Property(x => x.Username).HasMaxLength(50).IsRequired();
            entity.Property(x => x.PasswordHash).HasMaxLength(255).IsRequired();
            entity.Property(x => x.Role).HasMaxLength(20).IsRequired();
        });

        // ===== Categories =====
        modelBuilder.Entity<Category>(entity =>
        {
            entity.Property(x => x.Name).HasMaxLength(100).IsRequired();
            entity.Property(x => x.Description).HasMaxLength(255);

            // Nếu muốn tên danh mục không trùng:
            entity.HasIndex(x => x.Name).IsUnique();
        });

        // ===== Products =====
        modelBuilder.Entity<Product>(entity =>
        {
            entity.Property(x => x.Name).HasMaxLength(200).IsRequired();
            entity.Property(x => x.Description).HasMaxLength(1000);
            entity.Property(x => x.Price).HasColumnType("decimal(18,2)").IsRequired();

            entity.HasIndex(x => x.CategoryId);

            entity.HasOne(x => x.Category)
                  .WithMany(c => c.Products)
                  .HasForeignKey(x => x.CategoryId)
                  .OnDelete(DeleteBehavior.Restrict); // tránh xoá Category làm mất Product
        });

        // ===== Customers =====
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.Property(x => x.FullName).HasMaxLength(150).IsRequired();
            entity.Property(x => x.Phone).HasMaxLength(20);
            entity.Property(x => x.Email).HasMaxLength(255);
            entity.Property(x => x.Address).HasMaxLength(255);

            // Tuỳ chọn: email unique (nếu dùng email làm định danh)
            // entity.HasIndex(x => x.Email).IsUnique();
        });

        // ===== Orders =====
        modelBuilder.Entity<Order>(entity =>
        {
            entity.Property(x => x.OrderCode).HasMaxLength(30).IsRequired();
            entity.Property(x => x.Status).HasMaxLength(20).IsRequired();
            entity.Property(x => x.TotalAmount).HasColumnType("decimal(18,2)");

            entity.HasIndex(x => x.OrderCode).IsUnique();
            entity.HasIndex(x => x.CustomerId);

            entity.HasOne(x => x.Customer)
                  .WithMany(c => c.Orders)
                  .HasForeignKey(x => x.CustomerId)
                  .OnDelete(DeleteBehavior.Restrict); // tránh xoá Customer làm mất Orders
        });

        // ===== OrderItems =====
        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.Property(x => x.UnitPrice).HasColumnType("decimal(18,2)").IsRequired();
            entity.Property(x => x.LineTotal).HasColumnType("decimal(18,2)");

            // Tránh trùng 1 sản phẩm nhiều dòng trong cùng 1 đơn
            entity.HasIndex(x => new { x.OrderId, x.ProductId }).IsUnique();

            entity.HasOne(x => x.Order)
                  .WithMany(o => o.Items)
                  .HasForeignKey(x => x.OrderId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.Product)
                  .WithMany(p => p.OrderItems)
                  .HasForeignKey(x => x.ProductId)
                  .OnDelete(DeleteBehavior.Restrict); // tránh xoá Product khi đã nằm trong đơn
        });
    }
}

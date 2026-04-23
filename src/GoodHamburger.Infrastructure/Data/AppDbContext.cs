using GoodHamburger.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GoodHamburger.Infrastructure.Data;

public class AppDbContext(DbContextOptions Options) : DbContext(Options)
{
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    protected override void OnModelCreating(ModelBuilder ModelBuilder)
    {
        ModelBuilder.Entity<Order>(Builder =>
        {
            Builder.HasKey(X => X.Id);
            Builder.Property(X => X.TotalAmount).HasColumnType("decimal(10,2)");
            Builder.Property(X => X.DiscountPercent).HasColumnType("decimal(5,2)");
            Builder.HasMany(X => X.Items)
                .WithOne()
                .HasForeignKey("OrderId")
                .OnDelete(DeleteBehavior.Cascade);
        });
        
        ModelBuilder.Entity<OrderItem>(Builder =>
        {
            Builder.HasKey(X => X.Id);
            Builder.Property(X => X.Item).HasConversion<int>();
        });
    }
}
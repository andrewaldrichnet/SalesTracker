using Microsoft.EntityFrameworkCore;
using SalesTracker.Shared.Models;

namespace SalesTracker.Data.Sqlite;

public class SalesTrackerDbContext : DbContext
{
    public SalesTrackerDbContext(DbContextOptions<SalesTrackerDbContext> options) : base(options)
    {
    }

    public DbSet<Order> Orders { get; set; }
    public DbSet<Item> Items { get; set; }
    public DbSet<ItemImage> ItemImages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderID);
            entity.Property(e => e.CustomerName).IsRequired();
            entity.Property(e => e.Price).HasPrecision(18, 2);
            entity.Property(e => e.CreatedDate).HasDefaultValueSql("datetime('now')");
            entity.Property(e => e.ModifiedDate).HasDefaultValueSql("datetime('now')");
        });

        modelBuilder.Entity<Item>(entity =>
        {
            entity.HasKey(e => e.ItemID);
            entity.Property(e => e.Name).IsRequired();
            entity.Property(e => e.Cost).HasPrecision(18, 2).IsRequired();
            entity.Property(e => e.SalePrice).HasPrecision(18, 2);
            entity.Property(e => e.CreatedDate).HasDefaultValueSql("datetime('now')");
            entity.Property(e => e.ModifiedDate).HasDefaultValueSql("datetime('now')");
            entity.HasMany(e => e.Images).WithOne();
        });

        modelBuilder.Entity<ItemImage>(entity =>
        {
            entity.HasKey(e => e.ImageID);
            entity.Property(e => e.ImagePath).IsRequired();
            entity.HasOne<Item>().WithMany(i => i.Images).HasForeignKey(e => e.ItemID);
        });
    }
}

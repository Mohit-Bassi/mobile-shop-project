using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MobileShop.Domain.Entities;

namespace MobileShop.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Mobile> Mobiles => Set<Mobile>();
    public DbSet<Accessory> Accessories => Set<Accessory>();
    public DbSet<AccessoryCompatibleMobile> AccessoryCompatibleMobiles => Set<AccessoryCompatibleMobile>();
    public DbSet<RepairService> RepairServices => Set<RepairService>();
    public DbSet<Inquiry> Inquiries => Set<Inquiry>();
    public DbSet<Image> Images => Set<Image>();
    public DbSet<ImageVariant> ImageVariants => Set<ImageVariant>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        if (Database.ProviderName == "Microsoft.EntityFrameworkCore.Sqlite")
        {
            // SQLite has no native decimal type and can't ORDER BY the TEXT representation EF
            // Core stores decimals as by default. Tests run against SQLite for speed, so convert
            // decimal columns to double there; SQL Server (dev/prod) keeps native decimal(10,2).
            foreach (var property in modelBuilder.Model.GetEntityTypes()
                         .SelectMany(t => t.GetProperties())
                         .Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?)))
            {
                property.SetValueConverter(property.ClrType == typeof(decimal)
                    ? new ValueConverter<decimal, double>(v => (double)v, v => (decimal)v)
                    : new ValueConverter<decimal?, double?>(v => v == null ? null : (double)v, v => v == null ? null : (decimal)v));
            }
        }
    }
}

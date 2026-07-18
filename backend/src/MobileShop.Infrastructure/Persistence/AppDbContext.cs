using Microsoft.EntityFrameworkCore;
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
    }
}

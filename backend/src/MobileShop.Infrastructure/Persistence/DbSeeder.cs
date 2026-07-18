using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MobileShop.Domain.Entities;
using MobileShop.Domain.Enums;

namespace MobileShop.Infrastructure.Persistence;

public class DbSeeder
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly ILogger<DbSeeder> _logger;

    public DbSeeder(AppDbContext context, IConfiguration configuration, ILogger<DbSeeder> logger)
    {
        _context = context;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        await SeedAdminUserAsync();
        await SeedSampleDataAsync();
    }

    private async Task SeedAdminUserAsync()
    {
        if (_context.Users.Any())
        {
            return;
        }

        var email = _configuration["AdminSeed:Email"];
        var password = _configuration["AdminSeed:Password"];

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            _logger.LogWarning("AdminSeed:Email/AdminSeed:Password not configured — skipping admin user seed.");
            return;
        }

        var admin = new User
        {
            Email = email,
            Role = "Admin",
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow,
        };

        var hasher = new PasswordHasher<User>();
        admin.PasswordHash = hasher.HashPassword(admin, password);

        _context.Users.Add(admin);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Seeded admin user {Email}.", email);
    }

    private async Task SeedSampleDataAsync()
    {
        if (_context.Categories.Any())
        {
            return;
        }

        var cases = new Category { Name = "Cases", Slug = "cases", DisplayOrder = 1 };
        var chargers = new Category { Name = "Chargers", Slug = "chargers", DisplayOrder = 2 };
        var earphones = new Category { Name = "Earphones", Slug = "earphones", DisplayOrder = 3 };
        var screenProtectors = new Category { Name = "Screen Protectors", Slug = "screen-protectors", DisplayOrder = 4 };
        _context.Categories.AddRange(cases, chargers, earphones, screenProtectors);

        var now = DateTime.UtcNow;
        _context.Mobiles.AddRange(
            new Mobile
            {
                Brand = "Apple",
                Model = "iPhone 13",
                Storage = "128GB",
                Color = "Midnight",
                ConditionGrade = ConditionGrade.Good,
                Price = 32999,
                Description = "Well-maintained, minor scratches on back panel.",
                Status = ListingStatus.Active,
                CreatedAtUtc = now,
                UpdatedAtUtc = now,
            },
            new Mobile
            {
                Brand = "Samsung",
                Model = "Galaxy S22",
                Storage = "256GB",
                Color = "Phantom Black",
                ConditionGrade = ConditionGrade.LikeNew,
                Price = 28999,
                Description = "Barely used, comes with original box.",
                Status = ListingStatus.Active,
                CreatedAtUtc = now,
                UpdatedAtUtc = now,
            });

        _context.RepairServices.AddRange(
            new RepairService { Title = "Screen Replacement", Description = "For any brand/model.", PriceFrom = 1499, EstimatedTurnaround = "1-2 hours", DisplayOrder = 1 },
            new RepairService { Title = "Battery Replacement", Description = "Original & compatible batteries available.", PriceFrom = 999, EstimatedTurnaround = "30-45 mins", DisplayOrder = 2 },
            new RepairService { Title = "Water Damage Repair", Description = "Diagnosis and repair for water-damaged devices.", PriceFrom = null, EstimatedTurnaround = "1-3 days", DisplayOrder = 3 });

        await _context.SaveChangesAsync();

        _logger.LogInformation("Seeded sample categories, mobiles, and repair services.");
    }
}

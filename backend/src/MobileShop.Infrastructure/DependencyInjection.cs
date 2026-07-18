using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MobileShop.Application.Interfaces.Repositories;
using MobileShop.Infrastructure.Persistence;
using MobileShop.Infrastructure.Repositories;

namespace MobileShop.Infrastructure;

public static class DependencyInjection
{
    /// <summary>
    /// Registers the full infrastructure stack including a SQL Server-backed AppDbContext.
    /// Not used in the "Testing" environment — CustomWebApplicationFactory registers a
    /// SQLite-backed AppDbContext instead via <see cref="AddInfrastructureServices"/>, since
    /// registering two providers for the same DbContext type in one service collection conflicts.
    /// </summary>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        return services.AddInfrastructureServices();
    }

    /// <summary>
    /// Registers repositories and the seeder without a DbContext provider, so tests can supply
    /// their own (e.g. SQLite in-memory).
    /// </summary>
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddScoped<Persistence.DbSeeder>();

        services.AddScoped<IMobileRepository, MobileRepository>();
        services.AddScoped<IAccessoryRepository, AccessoryRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IRepairServiceRepository, RepairServiceRepository>();

        return services;
    }
}

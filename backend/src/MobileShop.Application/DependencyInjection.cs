using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using MobileShop.Application.Interfaces.Services;
using MobileShop.Application.Services;

namespace MobileShop.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IMobileService, MobileService>();
        services.AddScoped<IAccessoryService, AccessoryService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IRepairServiceService, RepairServiceService>();
        services.AddScoped<IInquiryService, InquiryService>();
        services.AddScoped<IDashboardService, DashboardService>();

        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        return services;
    }
}

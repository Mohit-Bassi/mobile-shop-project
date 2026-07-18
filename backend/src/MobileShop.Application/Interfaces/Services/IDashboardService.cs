using MobileShop.Application.DTOs.Dashboard;

namespace MobileShop.Application.Interfaces.Services;

public interface IDashboardService
{
    Task<DashboardSummaryDto> GetSummaryAsync(CancellationToken ct);
}

using MobileShop.Application.DTOs.Dashboard;

namespace MobileShop.Application.Interfaces.Repositories;

public interface IDashboardRepository
{
    Task<DashboardSummaryDto> GetSummaryAsync(CancellationToken ct);
}

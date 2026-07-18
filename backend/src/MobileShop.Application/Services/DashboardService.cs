using MobileShop.Application.DTOs.Dashboard;
using MobileShop.Application.Interfaces.Repositories;
using MobileShop.Application.Interfaces.Services;

namespace MobileShop.Application.Services;

public class DashboardService : IDashboardService
{
    private readonly IDashboardRepository _repository;

    public DashboardService(IDashboardRepository repository)
    {
        _repository = repository;
    }

    public Task<DashboardSummaryDto> GetSummaryAsync(CancellationToken ct) =>
        _repository.GetSummaryAsync(ct);
}

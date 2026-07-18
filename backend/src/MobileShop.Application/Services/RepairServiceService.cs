using MobileShop.Application.DTOs.RepairServices;
using MobileShop.Application.Interfaces.Repositories;
using MobileShop.Application.Interfaces.Services;

namespace MobileShop.Application.Services;

public class RepairServiceService : IRepairServiceService
{
    private readonly IRepairServiceRepository _repository;

    public RepairServiceService(IRepairServiceRepository repository)
    {
        _repository = repository;
    }

    public Task<List<RepairServiceDto>> GetActiveAsync(CancellationToken ct) =>
        _repository.GetActiveAsync(ct);

    public Task<RepairServiceDto?> GetActiveByIdAsync(int repairServiceId, CancellationToken ct) =>
        _repository.GetActiveByIdAsync(repairServiceId, ct);
}

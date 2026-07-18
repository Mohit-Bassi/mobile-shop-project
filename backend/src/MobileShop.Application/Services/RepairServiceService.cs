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

    public Task<List<RepairServiceDto>> GetAllAsync(CancellationToken ct) =>
        _repository.GetAllAsync(ct);

    public Task<RepairServiceDto?> GetByIdAsync(int repairServiceId, CancellationToken ct) =>
        _repository.GetByIdAsync(repairServiceId, ct);

    public Task<int> CreateAsync(AdminRepairServiceRequest request, CancellationToken ct) =>
        _repository.CreateAsync(request, ct);

    public Task<bool> UpdateAsync(int repairServiceId, AdminRepairServiceRequest request, CancellationToken ct) =>
        _repository.UpdateAsync(repairServiceId, request, ct);

    public Task<bool> DeleteAsync(int repairServiceId, CancellationToken ct) =>
        _repository.DeleteAsync(repairServiceId, ct);
}

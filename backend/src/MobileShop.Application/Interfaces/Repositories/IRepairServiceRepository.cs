using MobileShop.Application.DTOs.RepairServices;

namespace MobileShop.Application.Interfaces.Repositories;

public interface IRepairServiceRepository
{
    Task<List<RepairServiceDto>> GetActiveAsync(CancellationToken ct);
    Task<RepairServiceDto?> GetActiveByIdAsync(int repairServiceId, CancellationToken ct);
}

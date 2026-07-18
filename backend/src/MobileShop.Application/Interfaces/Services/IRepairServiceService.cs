using MobileShop.Application.DTOs.RepairServices;

namespace MobileShop.Application.Interfaces.Services;

public interface IRepairServiceService
{
    Task<List<RepairServiceDto>> GetActiveAsync(CancellationToken ct);
    Task<RepairServiceDto?> GetActiveByIdAsync(int repairServiceId, CancellationToken ct);
}

using MobileShop.Application.DTOs.RepairServices;

namespace MobileShop.Application.Interfaces.Services;

public interface IRepairServiceService
{
    Task<List<RepairServiceDto>> GetActiveAsync(CancellationToken ct);
    Task<RepairServiceDto?> GetActiveByIdAsync(int repairServiceId, CancellationToken ct);

    Task<List<RepairServiceDto>> GetAllAsync(CancellationToken ct);
    Task<RepairServiceDto?> GetByIdAsync(int repairServiceId, CancellationToken ct);
    Task<int> CreateAsync(AdminRepairServiceRequest request, CancellationToken ct);
    Task<bool> UpdateAsync(int repairServiceId, AdminRepairServiceRequest request, CancellationToken ct);
    Task<bool> DeleteAsync(int repairServiceId, CancellationToken ct);
}

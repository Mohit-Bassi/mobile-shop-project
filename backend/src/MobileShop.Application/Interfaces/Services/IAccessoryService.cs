using MobileShop.Application.DTOs.Accessories;
using MobileShop.Common.Pagination;

namespace MobileShop.Application.Interfaces.Services;

public interface IAccessoryService
{
    Task<PagedResult<AccessoryListItemDto>> GetActivePagedAsync(AccessoryQueryParameters query, CancellationToken ct);
    Task<AccessoryDetailDto?> GetActiveDetailByIdAsync(int accessoryId, CancellationToken ct);

    Task<PagedResult<AccessoryListItemDto>> GetAdminPagedAsync(AdminAccessoryQueryParameters query, CancellationToken ct);
    Task<AccessoryDetailDto?> GetAdminDetailByIdAsync(int accessoryId, CancellationToken ct);
    Task<int> CreateAsync(AdminAccessoryRequest request, CancellationToken ct);
    Task<bool> UpdateAsync(int accessoryId, AdminAccessoryRequest request, CancellationToken ct);
    Task<bool> UpdateStatusAsync(int accessoryId, string status, CancellationToken ct);
}

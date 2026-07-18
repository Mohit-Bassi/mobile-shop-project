using MobileShop.Application.DTOs.Accessories;
using MobileShop.Common.Pagination;

namespace MobileShop.Application.Interfaces.Repositories;

public interface IAccessoryRepository
{
    Task<PagedResult<AccessoryListItemDto>> GetActivePagedAsync(AccessoryQueryParameters query, CancellationToken ct);
    Task<AccessoryDetailDto?> GetActiveDetailByIdAsync(int accessoryId, CancellationToken ct);
}

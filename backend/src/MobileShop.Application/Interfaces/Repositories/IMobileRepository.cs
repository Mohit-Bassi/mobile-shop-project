using MobileShop.Application.DTOs.Mobiles;
using MobileShop.Common.Pagination;

namespace MobileShop.Application.Interfaces.Repositories;

public interface IMobileRepository
{
    Task<PagedResult<MobileListItemDto>> GetActivePagedAsync(MobileQueryParameters query, CancellationToken ct);
    Task<MobileDetailDto?> GetActiveDetailByIdAsync(int mobileId, CancellationToken ct);
}

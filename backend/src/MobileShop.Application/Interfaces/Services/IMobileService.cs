using MobileShop.Application.DTOs.Mobiles;
using MobileShop.Common.Pagination;

namespace MobileShop.Application.Interfaces.Services;

public interface IMobileService
{
    Task<PagedResult<MobileListItemDto>> GetActivePagedAsync(MobileQueryParameters query, CancellationToken ct);
    Task<MobileDetailDto?> GetActiveDetailByIdAsync(int mobileId, CancellationToken ct);
}

using MobileShop.Application.DTOs.Mobiles;
using MobileShop.Common.Pagination;

namespace MobileShop.Application.Interfaces.Repositories;

public interface IMobileRepository
{
    Task<PagedResult<MobileListItemDto>> GetActivePagedAsync(MobileQueryParameters query, CancellationToken ct);
    Task<MobileDetailDto?> GetActiveDetailByIdAsync(int mobileId, CancellationToken ct);

    Task<PagedResult<MobileListItemDto>> GetAdminPagedAsync(AdminMobileQueryParameters query, CancellationToken ct);
    Task<MobileDetailDto?> GetAdminDetailByIdAsync(int mobileId, CancellationToken ct);
    Task<int> CreateAsync(AdminMobileRequest request, CancellationToken ct);
    Task<bool> UpdateAsync(int mobileId, AdminMobileRequest request, CancellationToken ct);
    Task<bool> UpdateStatusAsync(int mobileId, string status, CancellationToken ct);
}

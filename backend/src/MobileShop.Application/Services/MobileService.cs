using MobileShop.Application.DTOs.Mobiles;
using MobileShop.Application.Interfaces.Repositories;
using MobileShop.Application.Interfaces.Services;
using MobileShop.Common.Pagination;

namespace MobileShop.Application.Services;

public class MobileService : IMobileService
{
    private readonly IMobileRepository _repository;

    public MobileService(IMobileRepository repository)
    {
        _repository = repository;
    }

    public Task<PagedResult<MobileListItemDto>> GetActivePagedAsync(MobileQueryParameters query, CancellationToken ct) =>
        _repository.GetActivePagedAsync(query, ct);

    public Task<MobileDetailDto?> GetActiveDetailByIdAsync(int mobileId, CancellationToken ct) =>
        _repository.GetActiveDetailByIdAsync(mobileId, ct);

    public Task<PagedResult<MobileListItemDto>> GetAdminPagedAsync(AdminMobileQueryParameters query, CancellationToken ct) =>
        _repository.GetAdminPagedAsync(query, ct);

    public Task<MobileDetailDto?> GetAdminDetailByIdAsync(int mobileId, CancellationToken ct) =>
        _repository.GetAdminDetailByIdAsync(mobileId, ct);

    public Task<int> CreateAsync(AdminMobileRequest request, CancellationToken ct) =>
        _repository.CreateAsync(request, ct);

    public Task<bool> UpdateAsync(int mobileId, AdminMobileRequest request, CancellationToken ct) =>
        _repository.UpdateAsync(mobileId, request, ct);

    public Task<bool> UpdateStatusAsync(int mobileId, string status, CancellationToken ct) =>
        _repository.UpdateStatusAsync(mobileId, status, ct);
}

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
}

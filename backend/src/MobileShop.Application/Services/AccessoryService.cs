using MobileShop.Application.DTOs.Accessories;
using MobileShop.Application.Interfaces.Repositories;
using MobileShop.Application.Interfaces.Services;
using MobileShop.Common.Pagination;

namespace MobileShop.Application.Services;

public class AccessoryService : IAccessoryService
{
    private readonly IAccessoryRepository _repository;

    public AccessoryService(IAccessoryRepository repository)
    {
        _repository = repository;
    }

    public Task<PagedResult<AccessoryListItemDto>> GetActivePagedAsync(AccessoryQueryParameters query, CancellationToken ct) =>
        _repository.GetActivePagedAsync(query, ct);

    public Task<AccessoryDetailDto?> GetActiveDetailByIdAsync(int accessoryId, CancellationToken ct) =>
        _repository.GetActiveDetailByIdAsync(accessoryId, ct);
}

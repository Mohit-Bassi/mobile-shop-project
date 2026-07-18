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

    public Task<PagedResult<AccessoryListItemDto>> GetAdminPagedAsync(AdminAccessoryQueryParameters query, CancellationToken ct) =>
        _repository.GetAdminPagedAsync(query, ct);

    public Task<AccessoryDetailDto?> GetAdminDetailByIdAsync(int accessoryId, CancellationToken ct) =>
        _repository.GetAdminDetailByIdAsync(accessoryId, ct);

    public Task<int> CreateAsync(AdminAccessoryRequest request, CancellationToken ct) =>
        _repository.CreateAsync(request, ct);

    public Task<bool> UpdateAsync(int accessoryId, AdminAccessoryRequest request, CancellationToken ct) =>
        _repository.UpdateAsync(accessoryId, request, ct);

    public Task<bool> UpdateStatusAsync(int accessoryId, string status, CancellationToken ct) =>
        _repository.UpdateStatusAsync(accessoryId, status, ct);
}

using Microsoft.EntityFrameworkCore;
using MobileShop.Application.DTOs.Accessories;
using MobileShop.Application.Interfaces.Repositories;
using MobileShop.Common.Pagination;
using MobileShop.Domain.Enums;
using MobileShop.Infrastructure.Persistence;

namespace MobileShop.Infrastructure.Repositories;

public class AccessoryRepository : IAccessoryRepository
{
    private readonly AppDbContext _context;

    public AccessoryRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<AccessoryListItemDto>> GetActivePagedAsync(AccessoryQueryParameters query, CancellationToken ct)
    {
        var accessories = _context.Accessories.AsNoTracking().Where(a => a.Status == ListingStatus.Active);

        if (query.CategoryId.HasValue)
        {
            accessories = accessories.Where(a => a.CategoryId == query.CategoryId.Value);
        }

        if (!string.IsNullOrWhiteSpace(query.CompatibleBrand) || !string.IsNullOrWhiteSpace(query.CompatibleModel))
        {
            accessories = accessories.Where(a => a.CompatibleMobiles.Any(cm =>
                (string.IsNullOrWhiteSpace(query.CompatibleBrand) || cm.CompatibleBrand == query.CompatibleBrand) &&
                (string.IsNullOrWhiteSpace(query.CompatibleModel) || cm.CompatibleModel == query.CompatibleModel)));
        }

        accessories = accessories.OrderByDescending(a => a.CreatedAtUtc);

        var totalCount = await accessories.CountAsync(ct);

        var items = await accessories
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(a => new AccessoryListItemDto
            {
                AccessoryId = a.AccessoryId,
                Name = a.Name,
                CategoryId = a.CategoryId,
                CategoryName = a.Category.Name,
                Price = a.Price,
                PrimaryImageId = _context.Images
                    .Where(i => i.OwnerType == ImageOwnerType.Accessory && i.OwnerId == a.AccessoryId && i.IsPrimary)
                    .Select(i => (int?)i.ImageId)
                    .FirstOrDefault(),
            })
            .ToListAsync(ct);

        return PagedResult<AccessoryListItemDto>.Create(items, query.Page, query.PageSize, totalCount);
    }

    public async Task<AccessoryDetailDto?> GetActiveDetailByIdAsync(int accessoryId, CancellationToken ct)
    {
        var accessory = await _context.Accessories.AsNoTracking()
            .Where(a => a.AccessoryId == accessoryId && a.Status == ListingStatus.Active)
            .Select(a => new AccessoryDetailDto
            {
                AccessoryId = a.AccessoryId,
                Name = a.Name,
                CategoryId = a.CategoryId,
                CategoryName = a.Category.Name,
                Price = a.Price,
                Description = a.Description,
                Status = a.Status.ToString(),
                CompatibleMobiles = a.CompatibleMobiles
                    .Select(cm => new CompatibleMobileDto { Brand = cm.CompatibleBrand, Model = cm.CompatibleModel })
                    .ToList(),
                ImageIds = _context.Images
                    .Where(i => i.OwnerType == ImageOwnerType.Accessory && i.OwnerId == a.AccessoryId)
                    .OrderBy(i => i.DisplayOrder)
                    .Select(i => i.ImageId)
                    .ToList(),
            })
            .FirstOrDefaultAsync(ct);

        return accessory;
    }
}

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
                Status = a.Status.ToString(),
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

    public async Task<PagedResult<AccessoryListItemDto>> GetAdminPagedAsync(AdminAccessoryQueryParameters query, CancellationToken ct)
    {
        var accessories = _context.Accessories.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Status) &&
            Enum.TryParse<ListingStatus>(query.Status, ignoreCase: true, out var status))
        {
            accessories = accessories.Where(a => a.Status == status);
        }

        if (query.CategoryId.HasValue)
        {
            accessories = accessories.Where(a => a.CategoryId == query.CategoryId.Value);
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
                Status = a.Status.ToString(),
                PrimaryImageId = _context.Images
                    .Where(i => i.OwnerType == ImageOwnerType.Accessory && i.OwnerId == a.AccessoryId && i.IsPrimary)
                    .Select(i => (int?)i.ImageId)
                    .FirstOrDefault(),
            })
            .ToListAsync(ct);

        return PagedResult<AccessoryListItemDto>.Create(items, query.Page, query.PageSize, totalCount);
    }

    public Task<AccessoryDetailDto?> GetAdminDetailByIdAsync(int accessoryId, CancellationToken ct) =>
        _context.Accessories.AsNoTracking()
            .Where(a => a.AccessoryId == accessoryId)
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
            .FirstOrDefaultAsync(ct)!;

    public async Task<int> CreateAsync(AdminAccessoryRequest request, CancellationToken ct)
    {
        var now = DateTime.UtcNow;
        var accessory = new Domain.Entities.Accessory
        {
            Name = request.Name,
            CategoryId = request.CategoryId,
            Price = request.Price,
            Description = request.Description,
            Status = Enum.Parse<ListingStatus>(request.Status, ignoreCase: true),
            CreatedAtUtc = now,
            UpdatedAtUtc = now,
            CompatibleMobiles = request.CompatibleMobiles
                .Select(cm => new Domain.Entities.AccessoryCompatibleMobile { CompatibleBrand = cm.Brand, CompatibleModel = cm.Model })
                .ToList(),
        };

        _context.Accessories.Add(accessory);
        await _context.SaveChangesAsync(ct);
        return accessory.AccessoryId;
    }

    public async Task<bool> UpdateAsync(int accessoryId, AdminAccessoryRequest request, CancellationToken ct)
    {
        var accessory = await _context.Accessories
            .Include(a => a.CompatibleMobiles)
            .FirstOrDefaultAsync(a => a.AccessoryId == accessoryId, ct);

        if (accessory is null)
        {
            return false;
        }

        accessory.Name = request.Name;
        accessory.CategoryId = request.CategoryId;
        accessory.Price = request.Price;
        accessory.Description = request.Description;
        accessory.Status = Enum.Parse<ListingStatus>(request.Status, ignoreCase: true);
        accessory.UpdatedAtUtc = DateTime.UtcNow;

        _context.AccessoryCompatibleMobiles.RemoveRange(accessory.CompatibleMobiles);
        accessory.CompatibleMobiles = request.CompatibleMobiles
            .Select(cm => new Domain.Entities.AccessoryCompatibleMobile { AccessoryId = accessoryId, CompatibleBrand = cm.Brand, CompatibleModel = cm.Model })
            .ToList();

        await _context.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> UpdateStatusAsync(int accessoryId, string status, CancellationToken ct)
    {
        if (!Enum.TryParse<ListingStatus>(status, ignoreCase: true, out var parsedStatus))
        {
            throw new Common.Exceptions.AppValidationException($"Invalid status '{status}'.");
        }

        var accessory = await _context.Accessories.FirstOrDefaultAsync(a => a.AccessoryId == accessoryId, ct);
        if (accessory is null)
        {
            return false;
        }

        accessory.Status = parsedStatus;
        accessory.UpdatedAtUtc = DateTime.UtcNow;
        await _context.SaveChangesAsync(ct);
        return true;
    }
}

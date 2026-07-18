using Microsoft.EntityFrameworkCore;
using MobileShop.Application.DTOs.Mobiles;
using MobileShop.Application.Interfaces.Repositories;
using MobileShop.Common.Pagination;
using MobileShop.Common.Sorting;
using MobileShop.Domain.Enums;
using MobileShop.Infrastructure.Persistence;

namespace MobileShop.Infrastructure.Repositories;

public class MobileRepository : IMobileRepository
{
    private static readonly Dictionary<string, string> AllowedSortFields = new(StringComparer.OrdinalIgnoreCase)
    {
        ["price"] = "Price",
        ["brand"] = "Brand",
        ["createdat"] = "CreatedAtUtc",
    };

    private readonly AppDbContext _context;

    public MobileRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<MobileListItemDto>> GetActivePagedAsync(MobileQueryParameters query, CancellationToken ct)
    {
        var mobiles = _context.Mobiles.AsNoTracking().Where(m => m.Status == ListingStatus.Active);

        if (!string.IsNullOrWhiteSpace(query.Brand))
        {
            mobiles = mobiles.Where(m => m.Brand == query.Brand);
        }

        if (query.MinPrice.HasValue)
        {
            mobiles = mobiles.Where(m => m.Price >= query.MinPrice.Value);
        }

        if (query.MaxPrice.HasValue)
        {
            mobiles = mobiles.Where(m => m.Price <= query.MaxPrice.Value);
        }

        if (!string.IsNullOrWhiteSpace(query.Condition) &&
            Enum.TryParse<ConditionGrade>(query.Condition, ignoreCase: true, out var condition))
        {
            mobiles = mobiles.Where(m => m.ConditionGrade == condition);
        }

        var sortOption = SortParser.Parse(query.Sort, AllowedSortFields);
        mobiles = sortOption switch
        {
            { Field: "Price", Descending: true } => mobiles.OrderByDescending(m => m.Price),
            { Field: "Price", Descending: false } => mobiles.OrderBy(m => m.Price),
            { Field: "Brand", Descending: true } => mobiles.OrderByDescending(m => m.Brand),
            { Field: "Brand", Descending: false } => mobiles.OrderBy(m => m.Brand),
            { Field: "CreatedAtUtc", Descending: true } => mobiles.OrderByDescending(m => m.CreatedAtUtc),
            { Field: "CreatedAtUtc", Descending: false } => mobiles.OrderBy(m => m.CreatedAtUtc),
            _ => mobiles.OrderByDescending(m => m.CreatedAtUtc),
        };

        var totalCount = await mobiles.CountAsync(ct);

        var items = await mobiles
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(m => new MobileListItemDto
            {
                MobileId = m.MobileId,
                Brand = m.Brand,
                Model = m.Model,
                Storage = m.Storage,
                Color = m.Color,
                ConditionGrade = m.ConditionGrade.ToString(),
                Price = m.Price,
                Status = m.Status.ToString(),
                PrimaryImageId = _context.Images
                    .Where(i => i.OwnerType == ImageOwnerType.Mobile && i.OwnerId == m.MobileId && i.IsPrimary)
                    .Select(i => (int?)i.ImageId)
                    .FirstOrDefault(),
            })
            .ToListAsync(ct);

        return PagedResult<MobileListItemDto>.Create(items, query.Page, query.PageSize, totalCount);
    }

    public async Task<MobileDetailDto?> GetActiveDetailByIdAsync(int mobileId, CancellationToken ct)
    {
        var mobile = await _context.Mobiles.AsNoTracking()
            .Where(m => m.MobileId == mobileId && m.Status == ListingStatus.Active)
            .Select(m => new MobileDetailDto
            {
                MobileId = m.MobileId,
                Brand = m.Brand,
                Model = m.Model,
                Storage = m.Storage,
                Color = m.Color,
                ConditionGrade = m.ConditionGrade.ToString(),
                Price = m.Price,
                Description = m.Description,
                SpecsJson = m.SpecsJson,
                Status = m.Status.ToString(),
                CreatedAtUtc = m.CreatedAtUtc,
                UpdatedAtUtc = m.UpdatedAtUtc,
                ImageIds = _context.Images
                    .Where(i => i.OwnerType == ImageOwnerType.Mobile && i.OwnerId == m.MobileId)
                    .OrderBy(i => i.DisplayOrder)
                    .Select(i => i.ImageId)
                    .ToList(),
            })
            .FirstOrDefaultAsync(ct);

        return mobile;
    }

    public async Task<PagedResult<MobileListItemDto>> GetAdminPagedAsync(AdminMobileQueryParameters query, CancellationToken ct)
    {
        var mobiles = _context.Mobiles.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Status) &&
            Enum.TryParse<ListingStatus>(query.Status, ignoreCase: true, out var status))
        {
            mobiles = mobiles.Where(m => m.Status == status);
        }

        if (!string.IsNullOrWhiteSpace(query.Brand))
        {
            mobiles = mobiles.Where(m => m.Brand == query.Brand);
        }

        mobiles = mobiles.OrderByDescending(m => m.CreatedAtUtc);

        var totalCount = await mobiles.CountAsync(ct);

        var items = await mobiles
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(m => new MobileListItemDto
            {
                MobileId = m.MobileId,
                Brand = m.Brand,
                Model = m.Model,
                Storage = m.Storage,
                Color = m.Color,
                ConditionGrade = m.ConditionGrade.ToString(),
                Price = m.Price,
                Status = m.Status.ToString(),
                PrimaryImageId = _context.Images
                    .Where(i => i.OwnerType == ImageOwnerType.Mobile && i.OwnerId == m.MobileId && i.IsPrimary)
                    .Select(i => (int?)i.ImageId)
                    .FirstOrDefault(),
            })
            .ToListAsync(ct);

        return PagedResult<MobileListItemDto>.Create(items, query.Page, query.PageSize, totalCount);
    }

    public Task<MobileDetailDto?> GetAdminDetailByIdAsync(int mobileId, CancellationToken ct) =>
        _context.Mobiles.AsNoTracking()
            .Where(m => m.MobileId == mobileId)
            .Select(m => new MobileDetailDto
            {
                MobileId = m.MobileId,
                Brand = m.Brand,
                Model = m.Model,
                Storage = m.Storage,
                Color = m.Color,
                ConditionGrade = m.ConditionGrade.ToString(),
                Price = m.Price,
                Description = m.Description,
                SpecsJson = m.SpecsJson,
                Status = m.Status.ToString(),
                CreatedAtUtc = m.CreatedAtUtc,
                UpdatedAtUtc = m.UpdatedAtUtc,
                ImageIds = _context.Images
                    .Where(i => i.OwnerType == ImageOwnerType.Mobile && i.OwnerId == m.MobileId)
                    .OrderBy(i => i.DisplayOrder)
                    .Select(i => i.ImageId)
                    .ToList(),
            })
            .FirstOrDefaultAsync(ct)!;

    public async Task<int> CreateAsync(AdminMobileRequest request, CancellationToken ct)
    {
        var now = DateTime.UtcNow;
        var mobile = new Domain.Entities.Mobile
        {
            Brand = request.Brand,
            Model = request.Model,
            Storage = request.Storage,
            Color = request.Color,
            ConditionGrade = Enum.Parse<ConditionGrade>(request.ConditionGrade, ignoreCase: true),
            Price = request.Price,
            Description = request.Description,
            SpecsJson = request.SpecsJson,
            Status = Enum.Parse<ListingStatus>(request.Status, ignoreCase: true),
            CreatedAtUtc = now,
            UpdatedAtUtc = now,
        };

        _context.Mobiles.Add(mobile);
        await _context.SaveChangesAsync(ct);
        return mobile.MobileId;
    }

    public async Task<bool> UpdateAsync(int mobileId, AdminMobileRequest request, CancellationToken ct)
    {
        var mobile = await _context.Mobiles.FirstOrDefaultAsync(m => m.MobileId == mobileId, ct);
        if (mobile is null)
        {
            return false;
        }

        mobile.Brand = request.Brand;
        mobile.Model = request.Model;
        mobile.Storage = request.Storage;
        mobile.Color = request.Color;
        mobile.ConditionGrade = Enum.Parse<ConditionGrade>(request.ConditionGrade, ignoreCase: true);
        mobile.Price = request.Price;
        mobile.Description = request.Description;
        mobile.SpecsJson = request.SpecsJson;
        mobile.Status = Enum.Parse<ListingStatus>(request.Status, ignoreCase: true);
        mobile.UpdatedAtUtc = DateTime.UtcNow;

        await _context.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> UpdateStatusAsync(int mobileId, string status, CancellationToken ct)
    {
        if (!Enum.TryParse<ListingStatus>(status, ignoreCase: true, out var parsedStatus))
        {
            throw new Common.Exceptions.AppValidationException($"Invalid status '{status}'.");
        }

        var mobile = await _context.Mobiles.FirstOrDefaultAsync(m => m.MobileId == mobileId, ct);
        if (mobile is null)
        {
            return false;
        }

        mobile.Status = parsedStatus;
        mobile.UpdatedAtUtc = DateTime.UtcNow;
        await _context.SaveChangesAsync(ct);
        return true;
    }
}

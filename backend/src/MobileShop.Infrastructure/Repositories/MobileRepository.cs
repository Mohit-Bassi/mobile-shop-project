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
}

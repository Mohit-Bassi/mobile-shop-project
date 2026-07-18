using Microsoft.EntityFrameworkCore;
using MobileShop.Application.DTOs.Categories;
using MobileShop.Application.Interfaces.Repositories;
using MobileShop.Infrastructure.Persistence;

namespace MobileShop.Infrastructure.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly AppDbContext _context;

    public CategoryRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<List<CategoryDto>> GetActiveAsync(CancellationToken ct) =>
        _context.Categories.AsNoTracking()
            .Where(c => c.IsActive)
            .OrderBy(c => c.DisplayOrder)
            .Select(c => new CategoryDto
            {
                CategoryId = c.CategoryId,
                Name = c.Name,
                Slug = c.Slug,
                DisplayOrder = c.DisplayOrder,
            })
            .ToListAsync(ct);
}

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
            .Select(c => new CategoryDto { CategoryId = c.CategoryId, Name = c.Name, Slug = c.Slug, DisplayOrder = c.DisplayOrder })
            .ToListAsync(ct);

    public Task<List<CategoryDto>> GetAllAsync(CancellationToken ct) =>
        _context.Categories.AsNoTracking()
            .OrderBy(c => c.DisplayOrder)
            .Select(c => new CategoryDto { CategoryId = c.CategoryId, Name = c.Name, Slug = c.Slug, DisplayOrder = c.DisplayOrder })
            .ToListAsync(ct);

    public Task<CategoryDto?> GetByIdAsync(int categoryId, CancellationToken ct) =>
        _context.Categories.AsNoTracking()
            .Where(c => c.CategoryId == categoryId)
            .Select(c => new CategoryDto { CategoryId = c.CategoryId, Name = c.Name, Slug = c.Slug, DisplayOrder = c.DisplayOrder })
            .FirstOrDefaultAsync(ct)!;

    public async Task<int> CreateAsync(AdminCategoryRequest request, CancellationToken ct)
    {
        var category = new Domain.Entities.Category
        {
            Name = request.Name,
            Slug = request.Slug,
            DisplayOrder = request.DisplayOrder,
            IsActive = request.IsActive,
        };

        _context.Categories.Add(category);
        await _context.SaveChangesAsync(ct);
        return category.CategoryId;
    }

    public async Task<bool> UpdateAsync(int categoryId, AdminCategoryRequest request, CancellationToken ct)
    {
        var category = await _context.Categories.FirstOrDefaultAsync(c => c.CategoryId == categoryId, ct);
        if (category is null)
        {
            return false;
        }

        category.Name = request.Name;
        category.Slug = request.Slug;
        category.DisplayOrder = request.DisplayOrder;
        category.IsActive = request.IsActive;

        await _context.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> DeleteAsync(int categoryId, CancellationToken ct)
    {
        // Soft-delete via IsActive: a hard delete would violate the FK from Accessories
        // (Restrict) whenever the category still has listings.
        var category = await _context.Categories.FirstOrDefaultAsync(c => c.CategoryId == categoryId, ct);
        if (category is null)
        {
            return false;
        }

        category.IsActive = false;
        await _context.SaveChangesAsync(ct);
        return true;
    }
}

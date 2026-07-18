using MobileShop.Application.DTOs.Categories;

namespace MobileShop.Application.Interfaces.Services;

public interface ICategoryService
{
    Task<List<CategoryDto>> GetActiveAsync(CancellationToken ct);

    Task<List<CategoryDto>> GetAllAsync(CancellationToken ct);
    Task<CategoryDto?> GetByIdAsync(int categoryId, CancellationToken ct);
    Task<int> CreateAsync(AdminCategoryRequest request, CancellationToken ct);
    Task<bool> UpdateAsync(int categoryId, AdminCategoryRequest request, CancellationToken ct);
    Task<bool> DeleteAsync(int categoryId, CancellationToken ct);
}

using MobileShop.Application.DTOs.Categories;

namespace MobileShop.Application.Interfaces.Services;

public interface ICategoryService
{
    Task<List<CategoryDto>> GetActiveAsync(CancellationToken ct);
}

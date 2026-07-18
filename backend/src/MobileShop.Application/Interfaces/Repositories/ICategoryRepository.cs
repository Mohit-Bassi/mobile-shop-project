using MobileShop.Application.DTOs.Categories;

namespace MobileShop.Application.Interfaces.Repositories;

public interface ICategoryRepository
{
    Task<List<CategoryDto>> GetActiveAsync(CancellationToken ct);
}

using MobileShop.Application.DTOs.Categories;
using MobileShop.Application.Interfaces.Repositories;
using MobileShop.Application.Interfaces.Services;

namespace MobileShop.Application.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _repository;

    public CategoryService(ICategoryRepository repository)
    {
        _repository = repository;
    }

    public Task<List<CategoryDto>> GetActiveAsync(CancellationToken ct) =>
        _repository.GetActiveAsync(ct);
}

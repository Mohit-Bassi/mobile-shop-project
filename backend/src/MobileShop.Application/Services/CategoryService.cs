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

    public Task<List<CategoryDto>> GetAllAsync(CancellationToken ct) =>
        _repository.GetAllAsync(ct);

    public Task<CategoryDto?> GetByIdAsync(int categoryId, CancellationToken ct) =>
        _repository.GetByIdAsync(categoryId, ct);

    public Task<int> CreateAsync(AdminCategoryRequest request, CancellationToken ct) =>
        _repository.CreateAsync(request, ct);

    public Task<bool> UpdateAsync(int categoryId, AdminCategoryRequest request, CancellationToken ct) =>
        _repository.UpdateAsync(categoryId, request, ct);

    public Task<bool> DeleteAsync(int categoryId, CancellationToken ct) =>
        _repository.DeleteAsync(categoryId, ct);
}

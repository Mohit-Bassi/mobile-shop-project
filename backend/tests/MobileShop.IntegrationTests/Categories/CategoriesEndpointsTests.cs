using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using MobileShop.Application.DTOs.Categories;
using MobileShop.Domain.Entities;
using MobileShop.Infrastructure.Persistence;

namespace MobileShop.IntegrationTests.Categories;

public class CategoriesEndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public CategoriesEndpointsTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetAll_ReturnsOnlyActiveCategoriesOrderedByDisplayOrder()
    {
        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            context.Categories.RemoveRange(context.Categories);
            context.Categories.AddRange(
                new Category { Name = "Chargers", Slug = "chargers", DisplayOrder = 2, IsActive = true },
                new Category { Name = "Cases", Slug = "cases", DisplayOrder = 1, IsActive = true },
                new Category { Name = "Discontinued", Slug = "discontinued", DisplayOrder = 0, IsActive = false });
            await context.SaveChangesAsync();
        }

        var client = _factory.CreateClient();
        var categories = await client.GetFromJsonAsync<List<CategoryDto>>("/api/v1/categories");

        categories!.Select(c => c.Name).Should().ContainInOrder("Cases", "Chargers");
        categories.Should().NotContain(c => c.Name == "Discontinued");
    }
}

using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MobileShop.Application.DTOs.Mobiles;
using MobileShop.Common.Pagination;
using MobileShop.Domain.Entities;
using MobileShop.Domain.Enums;
using MobileShop.Infrastructure.Persistence;

namespace MobileShop.IntegrationTests.Mobiles;

public class MobilesEndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public MobilesEndpointsTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    private async Task SeedAsync(params Mobile[] mobiles)
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        context.Mobiles.RemoveRange(context.Mobiles);
        await context.SaveChangesAsync();
        context.Mobiles.AddRange(mobiles);
        await context.SaveChangesAsync();
    }

    [Fact]
    public async Task GetPaged_ReturnsOnlyActiveMobiles()
    {
        var now = DateTime.UtcNow;
        await SeedAsync(
            new Mobile { Brand = "Apple", Model = "iPhone 13", ConditionGrade = ConditionGrade.Good, Price = 300, Status = ListingStatus.Active, CreatedAtUtc = now, UpdatedAtUtc = now },
            new Mobile { Brand = "Apple", Model = "iPhone 14", ConditionGrade = ConditionGrade.Good, Price = 400, Status = ListingStatus.Draft, CreatedAtUtc = now, UpdatedAtUtc = now });

        var client = _factory.CreateClient();
        var result = await client.GetFromJsonAsync<PagedResult<MobileListItemDto>>("/api/v1/mobiles");

        result!.Items.Should().ContainSingle(m => m.Model == "iPhone 13");
        result.Items.Should().NotContain(m => m.Model == "iPhone 14");
    }

    [Fact]
    public async Task GetPaged_FiltersByBrandAndSortsByPriceAscending()
    {
        var now = DateTime.UtcNow;
        await SeedAsync(
            new Mobile { Brand = "Samsung", Model = "S22", ConditionGrade = ConditionGrade.Good, Price = 500, Status = ListingStatus.Active, CreatedAtUtc = now, UpdatedAtUtc = now },
            new Mobile { Brand = "Apple", Model = "iPhone 13", ConditionGrade = ConditionGrade.Good, Price = 300, Status = ListingStatus.Active, CreatedAtUtc = now, UpdatedAtUtc = now },
            new Mobile { Brand = "Apple", Model = "iPhone 12", ConditionGrade = ConditionGrade.Fair, Price = 200, Status = ListingStatus.Active, CreatedAtUtc = now, UpdatedAtUtc = now });

        var client = _factory.CreateClient();
        var result = await client.GetFromJsonAsync<PagedResult<MobileListItemDto>>("/api/v1/mobiles?brand=Apple&sort=price_asc");

        result!.Items.Select(m => m.Model).Should().ContainInOrder("iPhone 12", "iPhone 13");
        result.TotalCount.Should().Be(2);
    }

    [Fact]
    public async Task GetPaged_RespectsPagination()
    {
        var now = DateTime.UtcNow;
        var mobiles = Enumerable.Range(1, 5)
            .Select(i => new Mobile { Brand = "Apple", Model = $"Model {i}", ConditionGrade = ConditionGrade.Good, Price = i * 100, Status = ListingStatus.Active, CreatedAtUtc = now, UpdatedAtUtc = now })
            .ToArray();
        await SeedAsync(mobiles);

        var client = _factory.CreateClient();
        var result = await client.GetFromJsonAsync<PagedResult<MobileListItemDto>>("/api/v1/mobiles?page=2&pageSize=2&sort=price_asc");

        result!.Items.Should().HaveCount(2);
        result.Page.Should().Be(2);
        result.TotalCount.Should().Be(5);
        result.TotalPages.Should().Be(3);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenMobileDoesNotExist()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/api/v1/mobiles/99999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenMobileIsNotActive()
    {
        var now = DateTime.UtcNow;
        await SeedAsync(new Mobile { Brand = "Apple", Model = "iPhone 13", ConditionGrade = ConditionGrade.Good, Price = 300, Status = ListingStatus.Draft, CreatedAtUtc = now, UpdatedAtUtc = now });

        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var mobileId = context.Mobiles.Single().MobileId;

        var client = _factory.CreateClient();
        var response = await client.GetAsync($"/api/v1/mobiles/{mobileId}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetById_ReturnsDetail_WhenMobileIsActive()
    {
        var now = DateTime.UtcNow;
        await SeedAsync(new Mobile { Brand = "Apple", Model = "iPhone 13", ConditionGrade = ConditionGrade.Good, Price = 300, Description = "Great phone", Status = ListingStatus.Active, CreatedAtUtc = now, UpdatedAtUtc = now });

        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var mobileId = context.Mobiles.Single().MobileId;

        var client = _factory.CreateClient();
        var detail = await client.GetFromJsonAsync<MobileDetailDto>($"/api/v1/mobiles/{mobileId}");

        detail!.Model.Should().Be("iPhone 13");
        detail.Description.Should().Be("Great phone");
    }
}

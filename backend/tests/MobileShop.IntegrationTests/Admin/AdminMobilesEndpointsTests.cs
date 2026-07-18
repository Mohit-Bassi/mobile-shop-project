using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MobileShop.Application.DTOs.Auth;
using MobileShop.Application.DTOs.Mobiles;
using MobileShop.Common.Pagination;
using MobileShop.Domain.Entities;
using MobileShop.Infrastructure.Persistence;

namespace MobileShop.IntegrationTests.Admin;

public class AdminMobilesEndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public AdminMobilesEndpointsTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    private async Task<HttpClient> CreateAuthenticatedClientAsync()
    {
        const string password = "Correct-Horse-Battery-Staple9!";
        var email = $"admin-{Guid.NewGuid():N}@test.local";

        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var user = new User { Email = email, Role = "Admin", IsActive = true, CreatedAtUtc = DateTime.UtcNow };
            user.PasswordHash = new PasswordHasher<User>().HashPassword(user, password);
            context.Users.Add(user);
            await context.SaveChangesAsync();
        }

        var client = _factory.CreateClient();
        var loginResponse = await client.PostAsJsonAsync("/api/v1/auth/login", new LoginRequest { Email = email, Password = password });
        var body = await loginResponse.Content.ReadFromJsonAsync<LoginResponseDto>();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", body!.AccessToken);
        return client;
    }

    [Fact]
    public async Task FullLifecycle_CreateReadUpdateStatusDelete()
    {
        var client = await CreateAuthenticatedClientAsync();

        var createRequest = new AdminMobileRequest
        {
            Brand = "TestBrand",
            Model = "TestModel",
            ConditionGrade = "Good",
            Price = 100,
            Status = "Draft",
        };

        var createResponse = await client.PostAsJsonAsync("/api/v1/admin/mobiles", createRequest);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = await createResponse.Content.ReadFromJsonAsync<Dictionary<string, int>>();
        var mobileId = created!["mobileId"];

        var getResponse = await client.GetAsync($"/api/v1/admin/mobiles/{mobileId}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var detail = await getResponse.Content.ReadFromJsonAsync<MobileDetailDto>();
        detail!.Status.Should().Be("Draft");

        // Draft items must not appear in the public catalog.
        var publicList = await client.GetFromJsonAsync<PagedResult<MobileListItemDto>>("/api/v1/mobiles?brand=TestBrand");
        publicList!.Items.Should().BeEmpty();

        var updateRequest = new AdminMobileRequest
        {
            Brand = "TestBrand",
            Model = "TestModel Updated",
            ConditionGrade = "LikeNew",
            Price = 150,
            Status = "Active",
        };
        var updateResponse = await client.PutAsJsonAsync($"/api/v1/admin/mobiles/{mobileId}", updateRequest);
        updateResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        publicList = await client.GetFromJsonAsync<PagedResult<MobileListItemDto>>("/api/v1/mobiles?brand=TestBrand");
        publicList!.Items.Should().ContainSingle(m => m.Model == "TestModel Updated");

        var statusResponse = await client.PatchAsJsonAsync($"/api/v1/admin/mobiles/{mobileId}/status", new { status = "SoldOut" });
        statusResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var deleteResponse = await client.DeleteAsync($"/api/v1/admin/mobiles/{mobileId}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var afterDelete = await client.GetFromJsonAsync<MobileDetailDto>($"/api/v1/admin/mobiles/{mobileId}");
        afterDelete!.Status.Should().Be("Draft");
    }

    [Fact]
    public async Task Create_ReturnsValidationProblem_ForInvalidRequest()
    {
        var client = await CreateAuthenticatedClientAsync();

        var response = await client.PostAsJsonAsync("/api/v1/admin/mobiles", new AdminMobileRequest
        {
            Brand = "",
            Model = "",
            ConditionGrade = "NotARealGrade",
            Price = -5,
            Status = "NotARealStatus",
        });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task AdminEndpoints_ReturnUnauthorized_WithoutToken()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/v1/admin/mobiles");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}

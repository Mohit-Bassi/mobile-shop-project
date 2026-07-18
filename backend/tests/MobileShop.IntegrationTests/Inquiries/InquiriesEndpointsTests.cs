using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using MobileShop.Application.DTOs.Auth;
using MobileShop.Application.DTOs.Inquiries;
using MobileShop.Common.Pagination;
using MobileShop.Domain.Entities;
using MobileShop.Infrastructure.Persistence;

namespace MobileShop.IntegrationTests.Inquiries;

public class InquiriesEndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public InquiriesEndpointsTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Submit_ReturnsCreated_ForValidRequest()
    {
        var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/v1/inquiries", new SubmitInquiryRequest
        {
            ListingType = "Mobile",
            ListingId = 1,
            CustomerName = "Jane Doe",
            CustomerPhone = "555-0101",
            CustomerEmail = "jane@example.com",
            Message = "Is this still available?",
        });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task Submit_ReturnsValidationProblem_ForMissingRequiredFields()
    {
        var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/v1/inquiries", new SubmitInquiryRequest
        {
            ListingType = "NotARealType",
            CustomerName = "",
            CustomerPhone = "",
        });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task SubmittedInquiry_IsVisibleInAdminEndpoint()
    {
        var client = _factory.CreateClient();

        var uniqueName = $"Customer-{Guid.NewGuid():N}";
        await client.PostAsJsonAsync("/api/v1/inquiries", new SubmitInquiryRequest
        {
            ListingType = "General",
            CustomerName = uniqueName,
            CustomerPhone = "555-0102",
        });

        // Admin login and list — confirms the public submission is surfaced to the admin dashboard.
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

        var adminClient = _factory.CreateClient();
        var loginResponse = await adminClient.PostAsJsonAsync("/api/v1/auth/login", new LoginRequest { Email = email, Password = password });
        var body = await loginResponse.Content.ReadFromJsonAsync<LoginResponseDto>();
        adminClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", body!.AccessToken);

        var adminList = await adminClient.GetFromJsonAsync<PagedResult<InquiryDto>>("/api/v1/admin/inquiries?pageSize=100");

        adminList!.Items.Should().Contain(i => i.CustomerName == uniqueName && i.Status == "New");
    }
}

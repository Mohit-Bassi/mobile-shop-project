using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MobileShop.Application.DTOs.Auth;
using MobileShop.Application.DTOs.Dashboard;
using MobileShop.Application.DTOs.Inquiries;
using MobileShop.Common.Pagination;
using MobileShop.Domain.Entities;
using MobileShop.Domain.Enums;
using MobileShop.Infrastructure.Persistence;

namespace MobileShop.IntegrationTests.Admin;

public class AdminInquiriesAndDashboardTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public AdminInquiriesAndDashboardTests(CustomWebApplicationFactory factory)
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

    private async Task<int> SeedInquiryAsync(InquiryStatus status)
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var inquiry = new Inquiry
        {
            ListingType = InquiryListingType.General,
            CustomerName = "Test Customer",
            CustomerPhone = "555-0100",
            Status = status,
            CreatedAtUtc = DateTime.UtcNow,
        };
        context.Inquiries.Add(inquiry);
        await context.SaveChangesAsync();
        return inquiry.InquiryId;
    }

    [Fact]
    public async Task GetPaged_ReturnsSeededInquiry_AndFiltersByStatus()
    {
        await SeedInquiryAsync(InquiryStatus.New);
        var client = await CreateAuthenticatedClientAsync();

        var result = await client.GetFromJsonAsync<PagedResult<InquiryDto>>("/api/v1/admin/inquiries?status=New");

        result!.Items.Should().Contain(i => i.CustomerName == "Test Customer" && i.Status == "New");
    }

    [Fact]
    public async Task UpdateStatus_ChangesInquiryStatus()
    {
        var inquiryId = await SeedInquiryAsync(InquiryStatus.New);
        var client = await CreateAuthenticatedClientAsync();

        var response = await client.PatchAsJsonAsync($"/api/v1/admin/inquiries/{inquiryId}/status", new { status = "Contacted" });
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var updated = await client.GetFromJsonAsync<InquiryDto>($"/api/v1/admin/inquiries/{inquiryId}");
        updated!.Status.Should().Be("Contacted");
    }

    [Fact]
    public async Task DashboardSummary_ReflectsCounts()
    {
        var client = await CreateAuthenticatedClientAsync();

        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            context.Inquiries.RemoveRange(context.Inquiries);
            context.Mobiles.RemoveRange(context.Mobiles);
            await context.SaveChangesAsync();

            var now = DateTime.UtcNow;
            context.Mobiles.Add(new Mobile { Brand = "A", Model = "B", ConditionGrade = ConditionGrade.Good, Price = 1, Status = ListingStatus.Active, CreatedAtUtc = now, UpdatedAtUtc = now });
            context.Inquiries.Add(new Inquiry { ListingType = InquiryListingType.General, CustomerName = "X", CustomerPhone = "1", Status = InquiryStatus.New, CreatedAtUtc = now });
            await context.SaveChangesAsync();
        }

        var summary = await client.GetFromJsonAsync<DashboardSummaryDto>("/api/v1/admin/dashboard/summary");

        summary!.ActiveMobiles.Should().Be(1);
        summary.NewInquiries.Should().Be(1);
        summary.TotalInquiries.Should().Be(1);
    }
}

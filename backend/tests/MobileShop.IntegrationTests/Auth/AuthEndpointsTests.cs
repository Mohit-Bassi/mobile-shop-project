using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MobileShop.Application.DTOs.Auth;
using MobileShop.Domain.Entities;
using MobileShop.Infrastructure.Persistence;

namespace MobileShop.IntegrationTests.Auth;

public class AuthEndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
    private const string TestPassword = "Correct-Horse-Battery-Staple9!";
    private readonly CustomWebApplicationFactory _factory;

    public AuthEndpointsTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    private async Task<string> SeedAdminUserAsync(string email)
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var user = new User { Email = email, Role = "Admin", IsActive = true, CreatedAtUtc = DateTime.UtcNow };
        user.PasswordHash = new PasswordHasher<User>().HashPassword(user, TestPassword);
        context.Users.Add(user);
        await context.SaveChangesAsync();
        return email;
    }

    private static HttpClient CreateClientWithCookies(CustomWebApplicationFactory factory, out CookieContainer cookieContainer)
    {
        cookieContainer = new CookieContainer();
        var client = factory.CreateDefaultClient(new DelegatingCookieHandler(cookieContainer, factory));
        // The refresh cookie is marked Secure; CookieContainer only attaches Secure cookies to
        // https:// requests, so the client (and cookie jar lookups) must use an https base
        // address even though TestServer's in-memory transport ignores the actual scheme.
        client.BaseAddress = new Uri("https://localhost");
        return client;
    }

    [Fact]
    public async Task Login_ReturnsAccessToken_ForValidCredentials()
    {
        var email = await SeedAdminUserAsync($"admin-{Guid.NewGuid():N}@test.local");
        var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/v1/auth/login", new LoginRequest { Email = email, Password = TestPassword });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<LoginResponseDto>();
        body!.AccessToken.Should().NotBeNullOrEmpty();
        response.Headers.TryGetValues("Set-Cookie", out var cookies).Should().BeTrue();
        cookies!.Should().Contain(c => c.StartsWith("refreshToken=") && c.Contains("httponly", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task Login_ReturnsUnauthorized_ForWrongPassword()
    {
        var email = await SeedAdminUserAsync($"admin-{Guid.NewGuid():N}@test.local");
        var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/v1/auth/login", new LoginRequest { Email = email, Password = "wrong-password" });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_ReturnsUnauthorized_ForUnknownEmail()
    {
        var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/v1/auth/login", new LoginRequest { Email = "nobody@test.local", Password = TestPassword });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ProtectedEndpoint_ReturnsUnauthorized_WithoutToken()
    {
        var client = _factory.CreateClient();

        var response = await client.DeleteAsync("/api/v1/admin/images/1");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ProtectedEndpoint_Succeeds_WithValidToken()
    {
        var email = await SeedAdminUserAsync($"admin-{Guid.NewGuid():N}@test.local");
        var client = _factory.CreateClient();

        var loginResponse = await client.PostAsJsonAsync("/api/v1/auth/login", new LoginRequest { Email = email, Password = TestPassword });
        var body = await loginResponse.Content.ReadFromJsonAsync<LoginResponseDto>();

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", body!.AccessToken);
        var response = await client.DeleteAsync("/api/v1/admin/images/999999");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Refresh_WithoutCookie_ReturnsUnauthorized()
    {
        var client = _factory.CreateClient();

        var response = await client.PostAsync("/api/v1/auth/refresh", content: null);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Refresh_RotatesToken_AndOldTokenBecomesInvalid()
    {
        var email = await SeedAdminUserAsync($"admin-{Guid.NewGuid():N}@test.local");
        var client = CreateClientWithCookies(_factory, out var cookieContainer);

        await client.PostAsJsonAsync("/api/v1/auth/login", new LoginRequest { Email = email, Password = TestPassword });
        var cookiesAfterLogin = cookieContainer.GetAllCookies();
        var originalRefreshToken = cookiesAfterLogin["refreshToken"]!.Value;

        var refreshResponse = await client.PostAsync("/api/v1/auth/refresh", content: null);
        refreshResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var rotatedToken = cookieContainer.GetAllCookies()["refreshToken"]!.Value;
        rotatedToken.Should().NotBe(originalRefreshToken);

        // Reuse the original (now-revoked) refresh token directly.
        using var reuseClient = _factory.CreateClient();
        reuseClient.DefaultRequestHeaders.Add("Cookie", $"refreshToken={originalRefreshToken}");
        var reuseResponse = await reuseClient.PostAsync("/api/v1/auth/refresh", content: null);

        reuseResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}

/// <summary>
/// TestServer's default client doesn't automatically follow the cookie container conventions of a
/// real browser, so this handler manually attaches/captures cookies for tests that need to
/// observe rotation across requests.
/// </summary>
internal class DelegatingCookieHandler : DelegatingHandler
{
    private readonly CookieContainer _cookieContainer;
    private readonly Uri _baseAddress;

    public DelegatingCookieHandler(CookieContainer cookieContainer, CustomWebApplicationFactory factory)
        : base(factory.Server.CreateHandler())
    {
        _cookieContainer = cookieContainer;
        // Must match the https:// base address the client uses, so CookieContainer treats the
        // Secure-flagged refresh cookie as eligible to send/store (see CreateClientWithCookies).
        _baseAddress = new Uri("https://localhost");
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // Use the actual request URI (not just the bare base address) so CookieContainer's
        // path-prefix matching against the cookie's Path=/api/v1/auth works correctly.
        var requestUri = request.RequestUri ?? _baseAddress;

        var cookieHeader = _cookieContainer.GetCookieHeader(requestUri);
        if (!string.IsNullOrEmpty(cookieHeader))
        {
            request.Headers.Add("Cookie", cookieHeader);
        }

        var response = await base.SendAsync(request, cancellationToken);

        if (response.Headers.TryGetValues("Set-Cookie", out var setCookieHeaders))
        {
            foreach (var header in setCookieHeaders)
            {
                _cookieContainer.SetCookies(requestUri, header);
            }
        }

        return response;
    }
}

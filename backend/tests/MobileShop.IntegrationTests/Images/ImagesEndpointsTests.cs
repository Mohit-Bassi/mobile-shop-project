using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MobileShop.Application.DTOs.Auth;
using MobileShop.Application.DTOs.Images;
using MobileShop.Domain.Entities;
using MobileShop.Domain.Enums;
using MobileShop.Infrastructure.Persistence;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace MobileShop.IntegrationTests.Images;

public class ImagesEndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public ImagesEndpointsTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    private static byte[] CreateSamplePngBytes()
    {
        using var image = new Image<Rgba32>(400, 300);
        using var stream = new MemoryStream();
        image.SaveAsPng(stream);
        return stream.ToArray();
    }

    private async Task<int> SeedMobileAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var now = DateTime.UtcNow;
        var mobile = new Mobile { Brand = "Apple", Model = "iPhone 13", ConditionGrade = ConditionGrade.Good, Price = 300, Status = ListingStatus.Active, CreatedAtUtc = now, UpdatedAtUtc = now };
        context.Mobiles.Add(mobile);
        await context.SaveChangesAsync();
        return mobile.MobileId;
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
    public async Task Upload_CreatesImageWithThreeVariants_AndFirstUploadIsPrimary()
    {
        var mobileId = await SeedMobileAsync();
        var client = await CreateAuthenticatedClientAsync();

        using var content = new MultipartFormDataContent();
        using var fileContent = new ByteArrayContent(CreateSamplePngBytes());
        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/png");
        content.Add(fileContent, "file", "sample.png");

        var response = await client.PostAsync($"/api/v1/admin/mobiles/{mobileId}/images", content);
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var result = await response.Content.ReadFromJsonAsync<ImageDto>();
        result!.IsPrimary.Should().BeTrue();

        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var variantCount = await context.ImageVariants.CountAsync(v => v.ImageId == result.ImageId);
        variantCount.Should().Be(3);
    }

    [Fact]
    public async Task GetVariant_ReturnsWebpBytesWithCacheHeaders()
    {
        var mobileId = await SeedMobileAsync();
        var client = await CreateAuthenticatedClientAsync();

        using var content = new MultipartFormDataContent();
        using var fileContent = new ByteArrayContent(CreateSamplePngBytes());
        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/png");
        content.Add(fileContent, "file", "sample.png");

        var uploadResponse = await client.PostAsync($"/api/v1/admin/mobiles/{mobileId}/images", content);
        var uploaded = await uploadResponse.Content.ReadFromJsonAsync<ImageDto>();

        var response = await client.GetAsync($"/api/v1/images/{uploaded!.ImageId}/thumbnail");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Headers.ContentType!.MediaType.Should().Be("image/webp");
        response.Headers.CacheControl!.Public.Should().BeTrue();
        response.Headers.ETag.Should().NotBeNull();

        var bytes = await response.Content.ReadAsByteArrayAsync();
        bytes.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetVariant_ReturnsNotFound_ForUnknownImage()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/api/v1/images/99999/full");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Upload_RejectsUnsupportedContentType()
    {
        var mobileId = await SeedMobileAsync();
        var client = await CreateAuthenticatedClientAsync();

        using var content = new MultipartFormDataContent();
        using var fileContent = new ByteArrayContent("not an image"u8.ToArray());
        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/plain");
        content.Add(fileContent, "file", "notanimage.txt");

        var response = await client.PostAsync($"/api/v1/admin/mobiles/{mobileId}/images", content);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Delete_ReassignsPrimaryToNextImage()
    {
        var mobileId = await SeedMobileAsync();
        var client = await CreateAuthenticatedClientAsync();

        async Task<ImageDto> UploadAsync()
        {
            using var content = new MultipartFormDataContent();
            using var fileContent = new ByteArrayContent(CreateSamplePngBytes());
            fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/png");
            content.Add(fileContent, "file", "sample.png");
            var resp = await client.PostAsync($"/api/v1/admin/mobiles/{mobileId}/images", content);
            return (await resp.Content.ReadFromJsonAsync<ImageDto>())!;
        }

        var first = await UploadAsync();
        var second = await UploadAsync();

        first.IsPrimary.Should().BeTrue();
        second.IsPrimary.Should().BeFalse();

        var deleteResponse = await client.DeleteAsync($"/api/v1/admin/images/{first.ImageId}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var remaining = await context.Images.SingleAsync(i => i.ImageId == second.ImageId);
        remaining.IsPrimary.Should().BeTrue();
    }
}

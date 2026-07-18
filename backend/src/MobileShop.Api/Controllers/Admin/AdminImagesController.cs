using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MobileShop.Application.DTOs.Images;
using MobileShop.Application.Interfaces.Services;
using MobileShop.Common.Exceptions;
using MobileShop.Domain.Enums;

namespace MobileShop.Api.Controllers.Admin;

[ApiController]
[Route("api/v1/admin")]
[Authorize(Roles = "Admin")]
public class AdminImagesController : ControllerBase
{
    private const long MaxUploadBytes = 8 * 1024 * 1024;

    private readonly IImageStorageService _imageStorageService;

    public AdminImagesController(IImageStorageService imageStorageService)
    {
        _imageStorageService = imageStorageService;
    }

    [HttpPost("mobiles/{mobileId:int}/images")]
    [RequestSizeLimit(MaxUploadBytes)]
    public Task<IActionResult> UploadMobileImage(int mobileId, IFormFile file, CancellationToken ct) =>
        UploadAsync(ImageOwnerType.Mobile, mobileId, file, ct);

    [HttpPost("accessories/{accessoryId:int}/images")]
    [RequestSizeLimit(MaxUploadBytes)]
    public Task<IActionResult> UploadAccessoryImage(int accessoryId, IFormFile file, CancellationToken ct) =>
        UploadAsync(ImageOwnerType.Accessory, accessoryId, file, ct);

    [HttpDelete("images/{imageId:int}")]
    public async Task<IActionResult> Delete(int imageId, CancellationToken ct)
    {
        await _imageStorageService.DeleteAsync(imageId, ct);
        return NoContent();
    }

    [HttpPatch("images/{imageId:int}/set-primary")]
    public async Task<IActionResult> SetPrimary(int imageId, CancellationToken ct)
    {
        try
        {
            await _imageStorageService.SetPrimaryAsync(imageId, ct);
            return NoContent();
        }
        catch (AppValidationException ex)
        {
            return ValidationProblem(ex.Message);
        }
    }

    [HttpPatch("mobiles/{mobileId:int}/images/reorder")]
    public Task<IActionResult> ReorderMobileImages(int mobileId, [FromBody] ReorderImagesRequest request, CancellationToken ct) =>
        ReorderAsync(ImageOwnerType.Mobile, mobileId, request, ct);

    [HttpPatch("accessories/{accessoryId:int}/images/reorder")]
    public Task<IActionResult> ReorderAccessoryImages(int accessoryId, [FromBody] ReorderImagesRequest request, CancellationToken ct) =>
        ReorderAsync(ImageOwnerType.Accessory, accessoryId, request, ct);

    private async Task<IActionResult> UploadAsync(ImageOwnerType ownerType, int ownerId, IFormFile? file, CancellationToken ct)
    {
        if (file is null || file.Length == 0)
        {
            return ValidationProblem("No file was uploaded.");
        }

        try
        {
            await using var stream = file.OpenReadStream();
            var result = await _imageStorageService.UploadAsync(ownerType, ownerId, stream, file.ContentType, ct);
            return CreatedAtAction(nameof(ImagesController.GetVariant), "Images", new { imageId = result.ImageId, variant = "full" }, result);
        }
        catch (AppValidationException ex)
        {
            return ValidationProblem(ex.Message);
        }
    }

    private async Task<IActionResult> ReorderAsync(ImageOwnerType ownerType, int ownerId, ReorderImagesRequest request, CancellationToken ct)
    {
        await _imageStorageService.ReorderAsync(ownerType, ownerId, request.ImageIds, ct);
        return NoContent();
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using MobileShop.Application.Interfaces.Services;
using MobileShop.Domain.Enums;

namespace MobileShop.Api.Controllers;

[ApiController]
[Route("api/v1/images")]
public class ImagesController : ControllerBase
{
    private readonly IImageStorageService _imageStorageService;

    public ImagesController(IImageStorageService imageStorageService)
    {
        _imageStorageService = imageStorageService;
    }

    [HttpGet("{imageId:int}/{variant}")]
    [OutputCache(Duration = 604_800)]
    public async Task<IActionResult> GetVariant(int imageId, string variant, CancellationToken ct)
    {
        if (!Enum.TryParse<ImageVariantType>(variant, ignoreCase: true, out var variantType))
        {
            return NotFound();
        }

        var result = await _imageStorageService.GetVariantAsync(imageId, variantType, ct);
        if (result is null)
        {
            return NotFound();
        }

        Response.Headers.CacheControl = "public, max-age=604800, immutable";
        Response.Headers.ETag = result.ETag;

        if (Request.Headers.IfNoneMatch == result.ETag)
        {
            return StatusCode(StatusCodes.Status304NotModified);
        }

        return File(result.Data, result.ContentType);
    }
}

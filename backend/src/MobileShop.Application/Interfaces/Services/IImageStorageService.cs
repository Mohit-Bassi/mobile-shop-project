using MobileShop.Application.DTOs.Images;
using MobileShop.Domain.Enums;

namespace MobileShop.Application.Interfaces.Services;

public interface IImageStorageService
{
    Task<ImageDto> UploadAsync(ImageOwnerType ownerType, int ownerId, Stream fileStream, string contentType, CancellationToken ct);
    Task<ImageVariantResult?> GetVariantAsync(int imageId, ImageVariantType variantType, CancellationToken ct);
    Task DeleteAsync(int imageId, CancellationToken ct);
    Task SetPrimaryAsync(int imageId, CancellationToken ct);
    Task ReorderAsync(ImageOwnerType ownerType, int ownerId, IReadOnlyList<int> orderedImageIds, CancellationToken ct);
}

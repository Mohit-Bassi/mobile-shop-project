using Microsoft.EntityFrameworkCore;
using MobileShop.Application.DTOs.Images;
using MobileShop.Application.Interfaces.Services;
using MobileShop.Common.Exceptions;
using MobileShop.Domain.Entities;
using MobileShop.Domain.Enums;
using MobileShop.Infrastructure.Persistence;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;
using ImageSharpImage = SixLabors.ImageSharp.Image;
using Size = SixLabors.ImageSharp.Size;

namespace MobileShop.Infrastructure.Services;

public class ImageStorageService : IImageStorageService
{
    private const long MaxUploadBytes = 8 * 1024 * 1024;

    private static readonly HashSet<string> AllowedContentTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "image/jpeg", "image/png", "image/webp",
    };

    private static readonly (ImageVariantType Type, int MaxDimension, int Quality)[] VariantSpecs =
    {
        (ImageVariantType.Thumbnail, 200, 75),
        (ImageVariantType.Medium, 800, 80),
        (ImageVariantType.Full, 1600, 85),
    };

    private readonly AppDbContext _context;

    public ImageStorageService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ImageDto> UploadAsync(ImageOwnerType ownerType, int ownerId, Stream fileStream, string contentType, CancellationToken ct)
    {
        if (!AllowedContentTypes.Contains(contentType))
        {
            throw new AppValidationException($"Unsupported image content type '{contentType}'. Allowed: image/jpeg, image/png, image/webp.");
        }

        using var buffer = new MemoryStream();
        await fileStream.CopyToAsync(buffer, ct);

        if (buffer.Length == 0)
        {
            throw new AppValidationException("Uploaded file is empty.");
        }

        if (buffer.Length > MaxUploadBytes)
        {
            throw new AppValidationException($"Uploaded file exceeds the maximum size of {MaxUploadBytes / (1024 * 1024)}MB.");
        }

        buffer.Position = 0;
        using var source = await ImageSharpImage.LoadAsync(buffer, ct);

        var hasExistingImages = await _context.Images
            .AnyAsync(i => i.OwnerType == ownerType && i.OwnerId == ownerId, ct);

        var maxDisplayOrder = await _context.Images
            .Where(i => i.OwnerType == ownerType && i.OwnerId == ownerId)
            .Select(i => (int?)i.DisplayOrder)
            .MaxAsync(ct) ?? -1;

        var image = new Image
        {
            OwnerType = ownerType,
            OwnerId = ownerId,
            IsPrimary = !hasExistingImages,
            DisplayOrder = maxDisplayOrder + 1,
            ContentType = "image/webp",
            SizeBytes = (int)buffer.Length,
            CreatedAtUtc = DateTime.UtcNow,
        };

        foreach (var (variantType, maxDimension, quality) in VariantSpecs)
        {
            using var clone = source.Clone(ctx => ctx.Resize(new ResizeOptions
            {
                Mode = ResizeMode.Max,
                Size = new Size(maxDimension, maxDimension),
            }));

            using var output = new MemoryStream();
            await clone.SaveAsync(output, new WebpEncoder { Quality = quality }, ct);

            image.Variants.Add(new ImageVariant
            {
                VariantType = variantType,
                Data = output.ToArray(),
                Width = clone.Width,
                Height = clone.Height,
            });
        }

        _context.Images.Add(image);
        await _context.SaveChangesAsync(ct);

        return new ImageDto { ImageId = image.ImageId, IsPrimary = image.IsPrimary, DisplayOrder = image.DisplayOrder };
    }

    public async Task<ImageVariantResult?> GetVariantAsync(int imageId, ImageVariantType variantType, CancellationToken ct)
    {
        var variant = await _context.ImageVariants.AsNoTracking()
            .Where(v => v.ImageId == imageId && v.VariantType == variantType)
            .Select(v => new { v.Data, Image = new { v.Image.ContentType } })
            .FirstOrDefaultAsync(ct);

        if (variant is null)
        {
            return null;
        }

        var eTag = $"\"{imageId}-{variantType}\"";
        return new ImageVariantResult { Data = variant.Data, ContentType = variant.Image.ContentType, ETag = eTag };
    }

    public async Task DeleteAsync(int imageId, CancellationToken ct)
    {
        var image = await _context.Images.FirstOrDefaultAsync(i => i.ImageId == imageId, ct);
        if (image is null)
        {
            return;
        }

        _context.Images.Remove(image);
        await _context.SaveChangesAsync(ct);

        if (image.IsPrimary)
        {
            var nextPrimary = await _context.Images
                .Where(i => i.OwnerType == image.OwnerType && i.OwnerId == image.OwnerId)
                .OrderBy(i => i.DisplayOrder)
                .FirstOrDefaultAsync(ct);

            if (nextPrimary is not null)
            {
                nextPrimary.IsPrimary = true;
                await _context.SaveChangesAsync(ct);
            }
        }
    }

    public async Task SetPrimaryAsync(int imageId, CancellationToken ct)
    {
        var image = await _context.Images.FirstOrDefaultAsync(i => i.ImageId == imageId, ct)
            ?? throw new AppValidationException($"Image {imageId} was not found.");

        var siblings = await _context.Images
            .Where(i => i.OwnerType == image.OwnerType && i.OwnerId == image.OwnerId)
            .ToListAsync(ct);

        foreach (var sibling in siblings)
        {
            sibling.IsPrimary = sibling.ImageId == imageId;
        }

        await _context.SaveChangesAsync(ct);
    }

    public async Task ReorderAsync(ImageOwnerType ownerType, int ownerId, IReadOnlyList<int> orderedImageIds, CancellationToken ct)
    {
        var images = await _context.Images
            .Where(i => i.OwnerType == ownerType && i.OwnerId == ownerId)
            .ToListAsync(ct);

        for (var i = 0; i < orderedImageIds.Count; i++)
        {
            var image = images.FirstOrDefault(x => x.ImageId == orderedImageIds[i]);
            if (image is not null)
            {
                image.DisplayOrder = i;
            }
        }

        await _context.SaveChangesAsync(ct);
    }
}

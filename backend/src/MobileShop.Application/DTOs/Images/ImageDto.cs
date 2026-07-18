namespace MobileShop.Application.DTOs.Images;

public class ImageDto
{
    public int ImageId { get; set; }
    public bool IsPrimary { get; set; }
    public int DisplayOrder { get; set; }
}

public class ImageVariantResult
{
    public byte[] Data { get; set; } = null!;
    public string ContentType { get; set; } = null!;
    public string ETag { get; set; } = null!;
}

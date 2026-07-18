using MobileShop.Domain.Enums;

namespace MobileShop.Domain.Entities;

public class ImageVariant
{
    public int ImageVariantId { get; set; }
    public int ImageId { get; set; }
    public ImageVariantType VariantType { get; set; }
    public byte[] Data { get; set; } = null!;
    public int Width { get; set; }
    public int Height { get; set; }

    public Image Image { get; set; } = null!;
}

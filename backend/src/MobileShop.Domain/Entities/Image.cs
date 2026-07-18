using MobileShop.Domain.Enums;

namespace MobileShop.Domain.Entities;

public class Image
{
    public int ImageId { get; set; }
    public ImageOwnerType OwnerType { get; set; }
    public int OwnerId { get; set; }
    public bool IsPrimary { get; set; }
    public int DisplayOrder { get; set; }
    public string ContentType { get; set; } = null!;
    public int SizeBytes { get; set; }
    public DateTime CreatedAtUtc { get; set; }

    public ICollection<ImageVariant> Variants { get; set; } = new List<ImageVariant>();
}

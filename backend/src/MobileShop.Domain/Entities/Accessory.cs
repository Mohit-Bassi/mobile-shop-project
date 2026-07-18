using MobileShop.Domain.Enums;

namespace MobileShop.Domain.Entities;

public class Accessory
{
    public int AccessoryId { get; set; }
    public string Name { get; set; } = null!;
    public int CategoryId { get; set; }
    public decimal Price { get; set; }
    public string? Description { get; set; }
    public ListingStatus Status { get; set; } = ListingStatus.Draft;
    public DateTime CreatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }

    public Category Category { get; set; } = null!;
    public ICollection<AccessoryCompatibleMobile> CompatibleMobiles { get; set; } = new List<AccessoryCompatibleMobile>();
}

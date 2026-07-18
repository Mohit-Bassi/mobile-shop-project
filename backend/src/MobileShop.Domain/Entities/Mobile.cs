using MobileShop.Domain.Enums;

namespace MobileShop.Domain.Entities;

public class Mobile
{
    public int MobileId { get; set; }
    public string Brand { get; set; } = null!;
    public string Model { get; set; } = null!;
    public string? Storage { get; set; }
    public string? Color { get; set; }
    public ConditionGrade ConditionGrade { get; set; }
    public decimal Price { get; set; }
    public string? Description { get; set; }
    public string? SpecsJson { get; set; }
    public ListingStatus Status { get; set; } = ListingStatus.Draft;
    public DateTime CreatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
}

namespace MobileShop.Application.DTOs.Mobiles;

public class MobileListItemDto
{
    public int MobileId { get; set; }
    public string Brand { get; set; } = null!;
    public string Model { get; set; } = null!;
    public string? Storage { get; set; }
    public string? Color { get; set; }
    public string ConditionGrade { get; set; } = null!;
    public decimal Price { get; set; }
    public int? PrimaryImageId { get; set; }
}

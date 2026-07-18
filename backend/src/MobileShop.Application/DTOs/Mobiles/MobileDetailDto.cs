namespace MobileShop.Application.DTOs.Mobiles;

public class MobileDetailDto
{
    public int MobileId { get; set; }
    public string Brand { get; set; } = null!;
    public string Model { get; set; } = null!;
    public string? Storage { get; set; }
    public string? Color { get; set; }
    public string ConditionGrade { get; set; } = null!;
    public decimal Price { get; set; }
    public string? Description { get; set; }
    public string? SpecsJson { get; set; }
    public string Status { get; set; } = null!;
    public DateTime CreatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
    public List<int> ImageIds { get; set; } = new();
}

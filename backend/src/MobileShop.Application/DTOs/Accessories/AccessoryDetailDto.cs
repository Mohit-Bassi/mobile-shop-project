namespace MobileShop.Application.DTOs.Accessories;

public class AccessoryDetailDto
{
    public int AccessoryId { get; set; }
    public string Name { get; set; } = null!;
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = null!;
    public decimal Price { get; set; }
    public string? Description { get; set; }
    public string Status { get; set; } = null!;
    public List<CompatibleMobileDto> CompatibleMobiles { get; set; } = new();
    public List<int> ImageIds { get; set; } = new();
}

public class CompatibleMobileDto
{
    public string Brand { get; set; } = null!;
    public string Model { get; set; } = null!;
}

namespace MobileShop.Application.DTOs.Accessories;

public class AccessoryListItemDto
{
    public int AccessoryId { get; set; }
    public string Name { get; set; } = null!;
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = null!;
    public decimal Price { get; set; }
    public string Status { get; set; } = null!;
    public int? PrimaryImageId { get; set; }
}

namespace MobileShop.Application.DTOs.Categories;

public class CategoryDto
{
    public int CategoryId { get; set; }
    public string Name { get; set; } = null!;
    public string Slug { get; set; } = null!;
    public int DisplayOrder { get; set; }
}

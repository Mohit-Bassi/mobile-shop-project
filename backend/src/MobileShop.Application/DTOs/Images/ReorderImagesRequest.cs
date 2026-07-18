namespace MobileShop.Application.DTOs.Images;

public class ReorderImagesRequest
{
    public List<int> ImageIds { get; set; } = new();
}

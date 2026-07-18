using MobileShop.Common.Pagination;

namespace MobileShop.Application.DTOs.Accessories;

public class AdminAccessoryRequest
{
    public string Name { get; set; } = null!;
    public int CategoryId { get; set; }
    public decimal Price { get; set; }
    public string? Description { get; set; }
    public string Status { get; set; } = "Draft";
    public List<CompatibleMobileDto> CompatibleMobiles { get; set; } = new();
}

public class AdminAccessoryQueryParameters : PageRequest
{
    public string? Status { get; set; }
    public int? CategoryId { get; set; }
}

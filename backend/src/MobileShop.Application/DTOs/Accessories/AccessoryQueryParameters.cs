using MobileShop.Common.Pagination;

namespace MobileShop.Application.DTOs.Accessories;

public class AccessoryQueryParameters : PageRequest
{
    public int? CategoryId { get; set; }
    public string? CompatibleBrand { get; set; }
    public string? CompatibleModel { get; set; }
}

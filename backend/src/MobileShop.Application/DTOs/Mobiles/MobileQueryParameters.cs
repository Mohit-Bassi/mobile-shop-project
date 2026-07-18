using MobileShop.Common.Pagination;

namespace MobileShop.Application.DTOs.Mobiles;

public class MobileQueryParameters : PageRequest
{
    public string? Brand { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public string? Condition { get; set; }
    public string? Sort { get; set; }
}

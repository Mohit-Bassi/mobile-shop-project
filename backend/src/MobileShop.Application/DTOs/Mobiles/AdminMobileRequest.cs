using MobileShop.Common.Pagination;

namespace MobileShop.Application.DTOs.Mobiles;

public class AdminMobileRequest
{
    public string Brand { get; set; } = null!;
    public string Model { get; set; } = null!;
    public string? Storage { get; set; }
    public string? Color { get; set; }
    public string ConditionGrade { get; set; } = null!;
    public decimal Price { get; set; }
    public string? Description { get; set; }
    public string? SpecsJson { get; set; }
    public string Status { get; set; } = "Draft";
}

public class AdminMobileQueryParameters : PageRequest
{
    public string? Status { get; set; }
    public string? Brand { get; set; }
}

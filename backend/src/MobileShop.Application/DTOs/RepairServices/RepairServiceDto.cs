namespace MobileShop.Application.DTOs.RepairServices;

public class RepairServiceDto
{
    public int RepairServiceId { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public decimal? PriceFrom { get; set; }
    public string? EstimatedTurnaround { get; set; }
    public int DisplayOrder { get; set; }
}

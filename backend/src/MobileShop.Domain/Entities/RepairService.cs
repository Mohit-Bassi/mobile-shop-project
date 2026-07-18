namespace MobileShop.Domain.Entities;

public class RepairService
{
    public int RepairServiceId { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public decimal? PriceFrom { get; set; }
    public string? EstimatedTurnaround { get; set; }
    public bool IsActive { get; set; } = true;
    public int DisplayOrder { get; set; }
}

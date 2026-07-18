using MobileShop.Domain.Enums;

namespace MobileShop.Domain.Entities;

public class Inquiry
{
    public int InquiryId { get; set; }
    public InquiryListingType ListingType { get; set; }
    public int? ListingId { get; set; }
    public string CustomerName { get; set; } = null!;
    public string CustomerPhone { get; set; } = null!;
    public string? CustomerEmail { get; set; }
    public string? Message { get; set; }
    public InquiryStatus Status { get; set; } = InquiryStatus.New;
    public DateTime CreatedAtUtc { get; set; }
}

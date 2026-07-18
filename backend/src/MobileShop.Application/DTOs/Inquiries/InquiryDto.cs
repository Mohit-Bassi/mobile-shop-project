using MobileShop.Common.Pagination;

namespace MobileShop.Application.DTOs.Inquiries;

public class InquiryDto
{
    public int InquiryId { get; set; }
    public string ListingType { get; set; } = null!;
    public int? ListingId { get; set; }
    public string CustomerName { get; set; } = null!;
    public string CustomerPhone { get; set; } = null!;
    public string? CustomerEmail { get; set; }
    public string? Message { get; set; }
    public string Status { get; set; } = null!;
    public DateTime CreatedAtUtc { get; set; }
}

public class SubmitInquiryRequest
{
    public string ListingType { get; set; } = null!;
    public int? ListingId { get; set; }
    public string CustomerName { get; set; } = null!;
    public string CustomerPhone { get; set; } = null!;
    public string? CustomerEmail { get; set; }
    public string? Message { get; set; }
}

public class InquiryQueryParameters : PageRequest
{
    public string? Status { get; set; }
}

public class UpdateInquiryStatusRequest
{
    public string Status { get; set; } = null!;
}

using MobileShop.Application.DTOs.Inquiries;
using MobileShop.Common.Pagination;

namespace MobileShop.Application.Interfaces.Services;

public interface IInquiryService
{
    Task<int> CreateAsync(SubmitInquiryRequest request, CancellationToken ct);
    Task<PagedResult<InquiryDto>> GetPagedAsync(InquiryQueryParameters query, CancellationToken ct);
    Task<InquiryDto?> GetByIdAsync(int inquiryId, CancellationToken ct);
    Task<bool> UpdateStatusAsync(int inquiryId, string status, CancellationToken ct);
}

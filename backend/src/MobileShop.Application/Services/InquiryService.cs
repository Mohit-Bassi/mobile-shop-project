using MobileShop.Application.DTOs.Inquiries;
using MobileShop.Application.Interfaces.Repositories;
using MobileShop.Application.Interfaces.Services;
using MobileShop.Common.Pagination;

namespace MobileShop.Application.Services;

public class InquiryService : IInquiryService
{
    private readonly IInquiryRepository _repository;

    public InquiryService(IInquiryRepository repository)
    {
        _repository = repository;
    }

    public Task<int> CreateAsync(SubmitInquiryRequest request, CancellationToken ct) =>
        _repository.CreateAsync(request, ct);

    public Task<PagedResult<InquiryDto>> GetPagedAsync(InquiryQueryParameters query, CancellationToken ct) =>
        _repository.GetPagedAsync(query, ct);

    public Task<InquiryDto?> GetByIdAsync(int inquiryId, CancellationToken ct) =>
        _repository.GetByIdAsync(inquiryId, ct);

    public Task<bool> UpdateStatusAsync(int inquiryId, string status, CancellationToken ct) =>
        _repository.UpdateStatusAsync(inquiryId, status, ct);
}

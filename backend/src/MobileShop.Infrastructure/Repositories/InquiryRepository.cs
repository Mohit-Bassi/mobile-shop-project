using Microsoft.EntityFrameworkCore;
using MobileShop.Application.DTOs.Inquiries;
using MobileShop.Application.Interfaces.Repositories;
using MobileShop.Common.Exceptions;
using MobileShop.Common.Pagination;
using MobileShop.Domain.Entities;
using MobileShop.Domain.Enums;
using MobileShop.Infrastructure.Persistence;

namespace MobileShop.Infrastructure.Repositories;

public class InquiryRepository : IInquiryRepository
{
    private readonly AppDbContext _context;

    public InquiryRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<int> CreateAsync(SubmitInquiryRequest request, CancellationToken ct)
    {
        if (!Enum.TryParse<InquiryListingType>(request.ListingType, ignoreCase: true, out var listingType))
        {
            throw new AppValidationException($"Invalid listingType '{request.ListingType}'.");
        }

        var inquiry = new Inquiry
        {
            ListingType = listingType,
            ListingId = request.ListingId,
            CustomerName = request.CustomerName,
            CustomerPhone = request.CustomerPhone,
            CustomerEmail = request.CustomerEmail,
            Message = request.Message,
            Status = InquiryStatus.New,
            CreatedAtUtc = DateTime.UtcNow,
        };

        _context.Inquiries.Add(inquiry);
        await _context.SaveChangesAsync(ct);
        return inquiry.InquiryId;
    }

    public async Task<PagedResult<InquiryDto>> GetPagedAsync(InquiryQueryParameters query, CancellationToken ct)
    {
        var inquiries = _context.Inquiries.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Status) &&
            Enum.TryParse<InquiryStatus>(query.Status, ignoreCase: true, out var status))
        {
            inquiries = inquiries.Where(i => i.Status == status);
        }

        inquiries = inquiries.OrderByDescending(i => i.CreatedAtUtc);

        var totalCount = await inquiries.CountAsync(ct);

        var items = await inquiries
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(i => new InquiryDto
            {
                InquiryId = i.InquiryId,
                ListingType = i.ListingType.ToString(),
                ListingId = i.ListingId,
                CustomerName = i.CustomerName,
                CustomerPhone = i.CustomerPhone,
                CustomerEmail = i.CustomerEmail,
                Message = i.Message,
                Status = i.Status.ToString(),
                CreatedAtUtc = i.CreatedAtUtc,
            })
            .ToListAsync(ct);

        return PagedResult<InquiryDto>.Create(items, query.Page, query.PageSize, totalCount);
    }

    public Task<InquiryDto?> GetByIdAsync(int inquiryId, CancellationToken ct) =>
        _context.Inquiries.AsNoTracking()
            .Where(i => i.InquiryId == inquiryId)
            .Select(i => new InquiryDto
            {
                InquiryId = i.InquiryId,
                ListingType = i.ListingType.ToString(),
                ListingId = i.ListingId,
                CustomerName = i.CustomerName,
                CustomerPhone = i.CustomerPhone,
                CustomerEmail = i.CustomerEmail,
                Message = i.Message,
                Status = i.Status.ToString(),
                CreatedAtUtc = i.CreatedAtUtc,
            })
            .FirstOrDefaultAsync(ct)!;

    public async Task<bool> UpdateStatusAsync(int inquiryId, string status, CancellationToken ct)
    {
        if (!Enum.TryParse<InquiryStatus>(status, ignoreCase: true, out var parsedStatus))
        {
            throw new AppValidationException($"Invalid status '{status}'.");
        }

        var inquiry = await _context.Inquiries.FirstOrDefaultAsync(i => i.InquiryId == inquiryId, ct);
        if (inquiry is null)
        {
            return false;
        }

        inquiry.Status = parsedStatus;
        await _context.SaveChangesAsync(ct);
        return true;
    }

    public Task<int> CountNewAsync(CancellationToken ct) =>
        _context.Inquiries.CountAsync(i => i.Status == InquiryStatus.New, ct);

    public Task<int> CountAllAsync(CancellationToken ct) =>
        _context.Inquiries.CountAsync(ct);
}

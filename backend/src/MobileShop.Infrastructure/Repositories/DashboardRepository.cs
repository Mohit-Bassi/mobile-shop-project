using Microsoft.EntityFrameworkCore;
using MobileShop.Application.DTOs.Dashboard;
using MobileShop.Application.Interfaces.Repositories;
using MobileShop.Domain.Enums;
using MobileShop.Infrastructure.Persistence;

namespace MobileShop.Infrastructure.Repositories;

public class DashboardRepository : IDashboardRepository
{
    private readonly AppDbContext _context;

    public DashboardRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<DashboardSummaryDto> GetSummaryAsync(CancellationToken ct)
    {
        return new DashboardSummaryDto
        {
            ActiveMobiles = await _context.Mobiles.CountAsync(m => m.Status == ListingStatus.Active, ct),
            ActiveAccessories = await _context.Accessories.CountAsync(a => a.Status == ListingStatus.Active, ct),
            NewInquiries = await _context.Inquiries.CountAsync(i => i.Status == InquiryStatus.New, ct),
            TotalInquiries = await _context.Inquiries.CountAsync(ct),
        };
    }
}

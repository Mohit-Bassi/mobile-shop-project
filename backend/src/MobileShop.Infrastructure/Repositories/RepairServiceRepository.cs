using Microsoft.EntityFrameworkCore;
using MobileShop.Application.DTOs.RepairServices;
using MobileShop.Application.Interfaces.Repositories;
using MobileShop.Infrastructure.Persistence;

namespace MobileShop.Infrastructure.Repositories;

public class RepairServiceRepository : IRepairServiceRepository
{
    private readonly AppDbContext _context;

    public RepairServiceRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<List<RepairServiceDto>> GetActiveAsync(CancellationToken ct) =>
        _context.RepairServices.AsNoTracking()
            .Where(rs => rs.IsActive)
            .OrderBy(rs => rs.DisplayOrder)
            .Select(rs => new RepairServiceDto
            {
                RepairServiceId = rs.RepairServiceId,
                Title = rs.Title,
                Description = rs.Description,
                PriceFrom = rs.PriceFrom,
                EstimatedTurnaround = rs.EstimatedTurnaround,
                DisplayOrder = rs.DisplayOrder,
            })
            .ToListAsync(ct);

    public Task<RepairServiceDto?> GetActiveByIdAsync(int repairServiceId, CancellationToken ct) =>
        _context.RepairServices.AsNoTracking()
            .Where(rs => rs.RepairServiceId == repairServiceId && rs.IsActive)
            .Select(rs => new RepairServiceDto
            {
                RepairServiceId = rs.RepairServiceId,
                Title = rs.Title,
                Description = rs.Description,
                PriceFrom = rs.PriceFrom,
                EstimatedTurnaround = rs.EstimatedTurnaround,
                DisplayOrder = rs.DisplayOrder,
            })
            .FirstOrDefaultAsync(ct)!;
}

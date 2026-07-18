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

    public Task<List<RepairServiceDto>> GetAllAsync(CancellationToken ct) =>
        _context.RepairServices.AsNoTracking()
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

    public Task<RepairServiceDto?> GetByIdAsync(int repairServiceId, CancellationToken ct) =>
        _context.RepairServices.AsNoTracking()
            .Where(rs => rs.RepairServiceId == repairServiceId)
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

    public async Task<int> CreateAsync(AdminRepairServiceRequest request, CancellationToken ct)
    {
        var repairService = new Domain.Entities.RepairService
        {
            Title = request.Title,
            Description = request.Description,
            PriceFrom = request.PriceFrom,
            EstimatedTurnaround = request.EstimatedTurnaround,
            IsActive = request.IsActive,
            DisplayOrder = request.DisplayOrder,
        };

        _context.RepairServices.Add(repairService);
        await _context.SaveChangesAsync(ct);
        return repairService.RepairServiceId;
    }

    public async Task<bool> UpdateAsync(int repairServiceId, AdminRepairServiceRequest request, CancellationToken ct)
    {
        var repairService = await _context.RepairServices.FirstOrDefaultAsync(rs => rs.RepairServiceId == repairServiceId, ct);
        if (repairService is null)
        {
            return false;
        }

        repairService.Title = request.Title;
        repairService.Description = request.Description;
        repairService.PriceFrom = request.PriceFrom;
        repairService.EstimatedTurnaround = request.EstimatedTurnaround;
        repairService.IsActive = request.IsActive;
        repairService.DisplayOrder = request.DisplayOrder;

        await _context.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> DeleteAsync(int repairServiceId, CancellationToken ct)
    {
        var repairService = await _context.RepairServices.FirstOrDefaultAsync(rs => rs.RepairServiceId == repairServiceId, ct);
        if (repairService is null)
        {
            return false;
        }

        repairService.IsActive = false;
        await _context.SaveChangesAsync(ct);
        return true;
    }
}

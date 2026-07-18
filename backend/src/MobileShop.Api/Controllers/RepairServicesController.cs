using Microsoft.AspNetCore.Mvc;
using MobileShop.Application.Interfaces.Services;

namespace MobileShop.Api.Controllers;

[ApiController]
[Route("api/v1/repair-services")]
public class RepairServicesController : ControllerBase
{
    private readonly IRepairServiceService _repairServiceService;

    public RepairServicesController(IRepairServiceService repairServiceService)
    {
        _repairServiceService = repairServiceService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var services = await _repairServiceService.GetActiveAsync(ct);
        return Ok(services);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        var service = await _repairServiceService.GetActiveByIdAsync(id, ct);
        return service is null ? NotFound() : Ok(service);
    }
}

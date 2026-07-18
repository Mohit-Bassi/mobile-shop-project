using Microsoft.AspNetCore.Mvc;
using MobileShop.Application.DTOs.Mobiles;
using MobileShop.Application.Interfaces.Services;

namespace MobileShop.Api.Controllers;

[ApiController]
[Route("api/v1/mobiles")]
public class MobilesController : ControllerBase
{
    private readonly IMobileService _mobileService;

    public MobilesController(IMobileService mobileService)
    {
        _mobileService = mobileService;
    }

    [HttpGet]
    public async Task<IActionResult> GetPaged([FromQuery] MobileQueryParameters query, CancellationToken ct)
    {
        var result = await _mobileService.GetActivePagedAsync(query, ct);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        var mobile = await _mobileService.GetActiveDetailByIdAsync(id, ct);
        return mobile is null ? NotFound() : Ok(mobile);
    }
}

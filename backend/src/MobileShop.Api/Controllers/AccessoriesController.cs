using Microsoft.AspNetCore.Mvc;
using MobileShop.Application.DTOs.Accessories;
using MobileShop.Application.Interfaces.Services;

namespace MobileShop.Api.Controllers;

[ApiController]
[Route("api/v1/accessories")]
public class AccessoriesController : ControllerBase
{
    private readonly IAccessoryService _accessoryService;

    public AccessoriesController(IAccessoryService accessoryService)
    {
        _accessoryService = accessoryService;
    }

    [HttpGet]
    public async Task<IActionResult> GetPaged([FromQuery] AccessoryQueryParameters query, CancellationToken ct)
    {
        var result = await _accessoryService.GetActivePagedAsync(query, ct);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        var accessory = await _accessoryService.GetActiveDetailByIdAsync(id, ct);
        return accessory is null ? NotFound() : Ok(accessory);
    }
}

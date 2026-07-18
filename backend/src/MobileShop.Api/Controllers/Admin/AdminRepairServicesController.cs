using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using MobileShop.Application.DTOs.RepairServices;
using MobileShop.Application.Interfaces.Services;

namespace MobileShop.Api.Controllers.Admin;

[ApiController]
[Route("api/v1/admin/repair-services")]
[Authorize(Roles = "Admin")]
public class AdminRepairServicesController : ControllerBase
{
    private readonly IRepairServiceService _repairServiceService;
    private readonly IValidator<AdminRepairServiceRequest> _validator;

    public AdminRepairServicesController(IRepairServiceService repairServiceService, IValidator<AdminRepairServiceRequest> validator)
    {
        _repairServiceService = repairServiceService;
        _validator = validator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var services = await _repairServiceService.GetAllAsync(ct);
        return Ok(services);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        var service = await _repairServiceService.GetByIdAsync(id, ct);
        return service is null ? NotFound() : Ok(service);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] AdminRepairServiceRequest request, CancellationToken ct)
    {
        var validation = await _validator.ValidateAsync(request, ct);
        if (!validation.IsValid)
        {
            return ValidationProblem(BuildModelState(validation));
        }

        var id = await _repairServiceService.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id }, new { repairServiceId = id });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] AdminRepairServiceRequest request, CancellationToken ct)
    {
        var validation = await _validator.ValidateAsync(request, ct);
        if (!validation.IsValid)
        {
            return ValidationProblem(BuildModelState(validation));
        }

        var updated = await _repairServiceService.UpdateAsync(id, request, ct);
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var deleted = await _repairServiceService.DeleteAsync(id, ct);
        return deleted ? NoContent() : NotFound();
    }

    private static ModelStateDictionary BuildModelState(FluentValidation.Results.ValidationResult validation)
    {
        var modelState = new ModelStateDictionary();
        foreach (var error in validation.Errors)
        {
            modelState.AddModelError(error.PropertyName, error.ErrorMessage);
        }

        return modelState;
    }
}

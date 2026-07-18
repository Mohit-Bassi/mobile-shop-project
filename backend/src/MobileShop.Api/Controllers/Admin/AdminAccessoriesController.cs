using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using MobileShop.Application.DTOs.Accessories;
using MobileShop.Application.DTOs.Common;
using MobileShop.Application.Interfaces.Services;
using MobileShop.Common.Exceptions;

namespace MobileShop.Api.Controllers.Admin;

[ApiController]
[Route("api/v1/admin/accessories")]
[Authorize(Roles = "Admin")]
public class AdminAccessoriesController : ControllerBase
{
    private readonly IAccessoryService _accessoryService;
    private readonly IValidator<AdminAccessoryRequest> _validator;

    public AdminAccessoriesController(IAccessoryService accessoryService, IValidator<AdminAccessoryRequest> validator)
    {
        _accessoryService = accessoryService;
        _validator = validator;
    }

    [HttpGet]
    public async Task<IActionResult> GetPaged([FromQuery] AdminAccessoryQueryParameters query, CancellationToken ct)
    {
        var result = await _accessoryService.GetAdminPagedAsync(query, ct);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        var accessory = await _accessoryService.GetAdminDetailByIdAsync(id, ct);
        return accessory is null ? NotFound() : Ok(accessory);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] AdminAccessoryRequest request, CancellationToken ct)
    {
        var validation = await _validator.ValidateAsync(request, ct);
        if (!validation.IsValid)
        {
            return ValidationProblem(BuildModelState(validation));
        }

        var id = await _accessoryService.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id }, new { accessoryId = id });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] AdminAccessoryRequest request, CancellationToken ct)
    {
        var validation = await _validator.ValidateAsync(request, ct);
        if (!validation.IsValid)
        {
            return ValidationProblem(BuildModelState(validation));
        }

        var updated = await _accessoryService.UpdateAsync(id, request, ct);
        return updated ? NoContent() : NotFound();
    }

    [HttpPatch("{id:int}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateStatusRequest request, CancellationToken ct)
    {
        try
        {
            var updated = await _accessoryService.UpdateStatusAsync(id, request.Status, ct);
            return updated ? NoContent() : NotFound();
        }
        catch (AppValidationException ex)
        {
            return ValidationProblem(ex.Message);
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var updated = await _accessoryService.UpdateStatusAsync(id, "Draft", ct);
        return updated ? NoContent() : NotFound();
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

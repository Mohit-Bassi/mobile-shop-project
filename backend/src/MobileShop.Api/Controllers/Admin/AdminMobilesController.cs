using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using MobileShop.Application.DTOs.Common;
using MobileShop.Application.DTOs.Mobiles;
using MobileShop.Application.Interfaces.Services;

namespace MobileShop.Api.Controllers.Admin;

[ApiController]
[Route("api/v1/admin/mobiles")]
[Authorize(Roles = "Admin")]
public class AdminMobilesController : ControllerBase
{
    private readonly IMobileService _mobileService;
    private readonly IValidator<AdminMobileRequest> _validator;

    public AdminMobilesController(IMobileService mobileService, IValidator<AdminMobileRequest> validator)
    {
        _mobileService = mobileService;
        _validator = validator;
    }

    [HttpGet]
    public async Task<IActionResult> GetPaged([FromQuery] AdminMobileQueryParameters query, CancellationToken ct)
    {
        var result = await _mobileService.GetAdminPagedAsync(query, ct);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        var mobile = await _mobileService.GetAdminDetailByIdAsync(id, ct);
        return mobile is null ? NotFound() : Ok(mobile);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] AdminMobileRequest request, CancellationToken ct)
    {
        var validation = await _validator.ValidateAsync(request, ct);
        if (!validation.IsValid)
        {
            return ValidationProblem(BuildModelState(validation));
        }

        var id = await _mobileService.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id }, new { mobileId = id });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] AdminMobileRequest request, CancellationToken ct)
    {
        var validation = await _validator.ValidateAsync(request, ct);
        if (!validation.IsValid)
        {
            return ValidationProblem(BuildModelState(validation));
        }

        var updated = await _mobileService.UpdateAsync(id, request, ct);
        return updated ? NoContent() : NotFound();
    }

    [HttpPatch("{id:int}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateStatusRequest request, CancellationToken ct)
    {
        try
        {
            var updated = await _mobileService.UpdateStatusAsync(id, request.Status, ct);
            return updated ? NoContent() : NotFound();
        }
        catch (MobileShop.Common.Exceptions.AppValidationException ex)
        {
            return ValidationProblem(ex.Message);
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        // Soft-delete: hide from the public catalog without discarding inventory history/images.
        var updated = await _mobileService.UpdateStatusAsync(id, "Draft", ct);
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

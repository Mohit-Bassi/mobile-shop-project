using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MobileShop.Application.DTOs.Inquiries;
using MobileShop.Application.Interfaces.Services;
using MobileShop.Common.Exceptions;

namespace MobileShop.Api.Controllers.Admin;

[ApiController]
[Route("api/v1/admin/inquiries")]
[Authorize(Roles = "Admin")]
public class AdminInquiriesController : ControllerBase
{
    private readonly IInquiryService _inquiryService;

    public AdminInquiriesController(IInquiryService inquiryService)
    {
        _inquiryService = inquiryService;
    }

    [HttpGet]
    public async Task<IActionResult> GetPaged([FromQuery] InquiryQueryParameters query, CancellationToken ct)
    {
        var result = await _inquiryService.GetPagedAsync(query, ct);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        var inquiry = await _inquiryService.GetByIdAsync(id, ct);
        return inquiry is null ? NotFound() : Ok(inquiry);
    }

    [HttpPatch("{id:int}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateInquiryStatusRequest request, CancellationToken ct)
    {
        try
        {
            var updated = await _inquiryService.UpdateStatusAsync(id, request.Status, ct);
            return updated ? NoContent() : NotFound();
        }
        catch (AppValidationException ex)
        {
            return ValidationProblem(ex.Message);
        }
    }
}

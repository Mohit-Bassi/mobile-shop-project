using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.RateLimiting;
using MobileShop.Application.DTOs.Inquiries;
using MobileShop.Application.Interfaces.Services;
using MobileShop.Common.Exceptions;

namespace MobileShop.Api.Controllers;

[ApiController]
[Route("api/v1/inquiries")]
public class InquiriesController : ControllerBase
{
    private readonly IInquiryService _inquiryService;
    private readonly IValidator<SubmitInquiryRequest> _validator;

    public InquiriesController(IInquiryService inquiryService, IValidator<SubmitInquiryRequest> validator)
    {
        _inquiryService = inquiryService;
        _validator = validator;
    }

    [HttpPost]
    [EnableRateLimiting("inquiries")]
    public async Task<IActionResult> Submit([FromBody] SubmitInquiryRequest request, CancellationToken ct)
    {
        var validation = await _validator.ValidateAsync(request, ct);
        if (!validation.IsValid)
        {
            var modelState = new ModelStateDictionary();
            foreach (var error in validation.Errors)
            {
                modelState.AddModelError(error.PropertyName, error.ErrorMessage);
            }

            return ValidationProblem(modelState);
        }

        try
        {
            var id = await _inquiryService.CreateAsync(request, ct);
            return CreatedAtAction(nameof(Submit), new { id }, new { inquiryId = id });
        }
        catch (AppValidationException ex)
        {
            return ValidationProblem(ex.Message);
        }
    }
}

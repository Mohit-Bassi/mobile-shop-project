using FluentValidation;
using MobileShop.Application.DTOs.RepairServices;

namespace MobileShop.Application.Validators;

public class AdminRepairServiceRequestValidator : AbstractValidator<AdminRepairServiceRequest>
{
    public AdminRepairServiceRequestValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(150);
        RuleFor(x => x.EstimatedTurnaround).MaximumLength(100);
        RuleFor(x => x.PriceFrom).GreaterThan(0).When(x => x.PriceFrom.HasValue);
    }
}

using FluentValidation;
using MobileShop.Application.DTOs.Mobiles;
using MobileShop.Domain.Enums;

namespace MobileShop.Application.Validators;

public class AdminMobileRequestValidator : AbstractValidator<AdminMobileRequest>
{
    public AdminMobileRequestValidator()
    {
        RuleFor(x => x.Brand).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Model).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Storage).MaximumLength(50);
        RuleFor(x => x.Color).MaximumLength(50);
        RuleFor(x => x.Price).GreaterThan(0);
        RuleFor(x => x.ConditionGrade).Must(v => Enum.TryParse<ConditionGrade>(v, ignoreCase: true, out _))
            .WithMessage("ConditionGrade must be one of: New, LikeNew, Good, Fair.");
        RuleFor(x => x.Status).Must(v => Enum.TryParse<ListingStatus>(v, ignoreCase: true, out _))
            .WithMessage("Status must be one of: Active, SoldOut, Draft.");
    }
}

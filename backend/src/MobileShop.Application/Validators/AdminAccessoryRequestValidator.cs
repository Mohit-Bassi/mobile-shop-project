using FluentValidation;
using MobileShop.Application.DTOs.Accessories;
using MobileShop.Domain.Enums;

namespace MobileShop.Application.Validators;

public class AdminAccessoryRequestValidator : AbstractValidator<AdminAccessoryRequest>
{
    public AdminAccessoryRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
        RuleFor(x => x.CategoryId).GreaterThan(0);
        RuleFor(x => x.Price).GreaterThan(0);
        RuleFor(x => x.Status).Must(v => Enum.TryParse<ListingStatus>(v, ignoreCase: true, out _))
            .WithMessage("Status must be one of: Active, SoldOut, Draft.");
    }
}

using FluentValidation;
using MobileShop.Application.DTOs.Categories;

namespace MobileShop.Application.Validators;

public class AdminCategoryRequestValidator : AbstractValidator<AdminCategoryRequest>
{
    public AdminCategoryRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Slug).NotEmpty().MaximumLength(120).Matches("^[a-z0-9]+(-[a-z0-9]+)*$")
            .WithMessage("Slug must be lowercase, alphanumeric, and hyphen-separated.");
    }
}

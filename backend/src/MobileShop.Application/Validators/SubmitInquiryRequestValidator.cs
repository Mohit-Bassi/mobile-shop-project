using FluentValidation;
using MobileShop.Application.DTOs.Inquiries;
using MobileShop.Domain.Enums;

namespace MobileShop.Application.Validators;

public class SubmitInquiryRequestValidator : AbstractValidator<SubmitInquiryRequest>
{
    public SubmitInquiryRequestValidator()
    {
        RuleFor(x => x.ListingType).Must(v => Enum.TryParse<InquiryListingType>(v, ignoreCase: true, out _))
            .WithMessage("listingType must be one of: Mobile, Accessory, RepairService, General.");
        RuleFor(x => x.CustomerName).NotEmpty().MaximumLength(150);
        RuleFor(x => x.CustomerPhone).NotEmpty().MaximumLength(30);
        RuleFor(x => x.CustomerEmail).EmailAddress().MaximumLength(256).When(x => !string.IsNullOrEmpty(x.CustomerEmail));
        RuleFor(x => x.Message).MaximumLength(1000);
    }
}

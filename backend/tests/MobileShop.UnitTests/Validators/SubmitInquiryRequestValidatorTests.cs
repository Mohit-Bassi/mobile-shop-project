using FluentAssertions;
using FluentValidation.TestHelper;
using MobileShop.Application.DTOs.Inquiries;
using MobileShop.Application.Validators;

namespace MobileShop.UnitTests.Validators;

public class SubmitInquiryRequestValidatorTests
{
    private readonly SubmitInquiryRequestValidator _validator = new();

    [Fact]
    public void Validate_Passes_ForMinimalValidRequest()
    {
        var request = new SubmitInquiryRequest { ListingType = "General", CustomerName = "Jane", CustomerPhone = "555-0100" };

        var result = _validator.TestValidate(request);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_Fails_ForInvalidListingType()
    {
        var request = new SubmitInquiryRequest { ListingType = "Car", CustomerName = "Jane", CustomerPhone = "555-0100" };

        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.ListingType);
    }

    [Fact]
    public void Validate_Fails_ForMissingNameOrPhone()
    {
        var request = new SubmitInquiryRequest { ListingType = "General", CustomerName = "", CustomerPhone = "" };

        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.CustomerName);
        result.ShouldHaveValidationErrorFor(x => x.CustomerPhone);
    }

    [Fact]
    public void Validate_Fails_ForInvalidEmail()
    {
        var request = new SubmitInquiryRequest { ListingType = "General", CustomerName = "Jane", CustomerPhone = "555-0100", CustomerEmail = "not-an-email" };

        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.CustomerEmail);
    }
}

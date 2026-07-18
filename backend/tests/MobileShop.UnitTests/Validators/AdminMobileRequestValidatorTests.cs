using FluentAssertions;
using FluentValidation.TestHelper;
using MobileShop.Application.DTOs.Mobiles;
using MobileShop.Application.Validators;

namespace MobileShop.UnitTests.Validators;

public class AdminMobileRequestValidatorTests
{
    private readonly AdminMobileRequestValidator _validator = new();

    private static AdminMobileRequest ValidRequest() => new()
    {
        Brand = "Apple",
        Model = "iPhone 13",
        ConditionGrade = "Good",
        Price = 300,
        Status = "Active",
    };

    [Fact]
    public void Validate_Passes_ForValidRequest()
    {
        var result = _validator.TestValidate(ValidRequest());
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_Fails_ForZeroPrice()
    {
        var request = ValidRequest();
        request.Price = 0;

        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Price);
    }

    [Fact]
    public void Validate_Fails_ForInvalidConditionGrade()
    {
        var request = ValidRequest();
        request.ConditionGrade = "Excellent";

        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.ConditionGrade);
    }

    [Fact]
    public void Validate_Fails_ForInvalidStatus()
    {
        var request = ValidRequest();
        request.Status = "Deleted";

        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Status);
    }

    [Fact]
    public void Validate_Fails_ForEmptyBrandOrModel()
    {
        var request = ValidRequest();
        request.Brand = "";
        request.Model = "";

        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Brand);
        result.ShouldHaveValidationErrorFor(x => x.Model);
    }
}

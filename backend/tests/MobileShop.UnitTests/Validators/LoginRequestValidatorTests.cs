using FluentAssertions;
using FluentValidation.TestHelper;
using MobileShop.Application.DTOs.Auth;
using MobileShop.Application.Validators;

namespace MobileShop.UnitTests.Validators;

public class LoginRequestValidatorTests
{
    private readonly LoginRequestValidator _validator = new();

    [Fact]
    public void Validate_Passes_ForValidRequest()
    {
        var result = _validator.TestValidate(new LoginRequest { Email = "admin@mobileshop.local", Password = "some-password" });
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("", "password")]
    [InlineData("not-an-email", "password")]
    [InlineData("admin@mobileshop.local", "")]
    public void Validate_Fails_ForInvalidInput(string email, string password)
    {
        var result = _validator.TestValidate(new LoginRequest { Email = email, Password = password });
        result.IsValid.Should().BeFalse();
    }
}

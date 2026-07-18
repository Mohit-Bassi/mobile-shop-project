namespace MobileShop.Common.Exceptions;

/// <summary>
/// Thrown for business-rule validation failures that should surface as HTTP 400.
/// </summary>
public class AppValidationException : Exception
{
    public AppValidationException(string message) : base(message)
    {
    }
}

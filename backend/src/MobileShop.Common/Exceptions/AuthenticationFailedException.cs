namespace MobileShop.Common.Exceptions;

/// <summary>
/// Thrown for login/refresh failures that should surface as HTTP 401, without leaking whether
/// the email, password, or token was the specific problem.
/// </summary>
public class AuthenticationFailedException : Exception
{
    public AuthenticationFailedException(string message) : base(message)
    {
    }
}

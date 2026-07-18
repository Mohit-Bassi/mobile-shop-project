namespace MobileShop.Application.DTOs.Auth;

/// <summary>
/// Internal service-layer result carrying the plaintext refresh token so the API layer can set
/// it as an httpOnly cookie. Never serialized directly to a client response.
/// </summary>
public class AuthResult
{
    public string AccessToken { get; set; } = null!;
    public DateTime AccessTokenExpiresAtUtc { get; set; }
    public string RefreshToken { get; set; } = null!;
    public DateTime RefreshTokenExpiresAtUtc { get; set; }
}

public class LoginResponseDto
{
    public string AccessToken { get; set; } = null!;
    public DateTime ExpiresAtUtc { get; set; }
}

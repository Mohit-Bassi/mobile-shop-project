using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using MobileShop.Application.DTOs.Auth;
using MobileShop.Application.Interfaces.Services;
using MobileShop.Common.Exceptions;

namespace MobileShop.Api.Controllers;

[ApiController]
[Route("api/v1/auth")]
public class AuthController : ControllerBase
{
    private const string RefreshTokenCookieName = "refreshToken";

    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    [EnableRateLimiting("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken ct)
    {
        try
        {
            var result = await _authService.LoginAsync(request.Email, request.Password, ct);
            SetRefreshTokenCookie(result.RefreshToken, result.RefreshTokenExpiresAtUtc);
            return Ok(new LoginResponseDto { AccessToken = result.AccessToken, ExpiresAtUtc = result.AccessTokenExpiresAtUtc });
        }
        catch (AuthenticationFailedException ex)
        {
            return Problem(detail: ex.Message, statusCode: StatusCodes.Status401Unauthorized);
        }
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(CancellationToken ct)
    {
        if (!Request.Cookies.TryGetValue(RefreshTokenCookieName, out var refreshToken) || string.IsNullOrEmpty(refreshToken))
        {
            return Problem(detail: "No refresh token was provided.", statusCode: StatusCodes.Status401Unauthorized);
        }

        try
        {
            var result = await _authService.RefreshAsync(refreshToken, ct);
            SetRefreshTokenCookie(result.RefreshToken, result.RefreshTokenExpiresAtUtc);
            return Ok(new LoginResponseDto { AccessToken = result.AccessToken, ExpiresAtUtc = result.AccessTokenExpiresAtUtc });
        }
        catch (AuthenticationFailedException ex)
        {
            Response.Cookies.Delete(RefreshTokenCookieName);
            return Problem(detail: ex.Message, statusCode: StatusCodes.Status401Unauthorized);
        }
    }

    [HttpPost("logout")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Logout(CancellationToken ct)
    {
        if (Request.Cookies.TryGetValue(RefreshTokenCookieName, out var refreshToken) && !string.IsNullOrEmpty(refreshToken))
        {
            await _authService.LogoutAsync(refreshToken, ct);
        }

        Response.Cookies.Delete(RefreshTokenCookieName);
        return NoContent();
    }

    private void SetRefreshTokenCookie(string token, DateTime expiresAtUtc)
    {
        Response.Cookies.Append(RefreshTokenCookieName, token, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = expiresAtUtc,
            Path = "/api/v1/auth",
        });
    }
}

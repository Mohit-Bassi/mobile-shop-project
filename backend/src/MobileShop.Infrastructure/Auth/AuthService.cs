using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MobileShop.Application.DTOs.Auth;
using MobileShop.Application.Interfaces.Services;
using MobileShop.Common.Exceptions;
using MobileShop.Domain.Entities;
using MobileShop.Infrastructure.Persistence;

namespace MobileShop.Infrastructure.Auth;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly JwtTokenService _jwtTokenService;
    private readonly IConfiguration _configuration;
    private readonly PasswordHasher<User> _passwordHasher = new();

    public AuthService(AppDbContext context, JwtTokenService jwtTokenService, IConfiguration configuration)
    {
        _context = context;
        _jwtTokenService = jwtTokenService;
        _configuration = configuration;
    }

    public async Task<AuthResult> LoginAsync(string email, string password, CancellationToken ct)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email, ct);

        // Deliberately generic message on every failure branch below, so responses don't
        // reveal whether the email exists, the account is locked, or the password was wrong.
        const string genericFailureMessage = "Invalid email or password.";

        if (user is null || !user.IsActive)
        {
            throw new AuthenticationFailedException(genericFailureMessage);
        }

        if (user.LockoutUntilUtc is not null && user.LockoutUntilUtc > DateTime.UtcNow)
        {
            throw new AuthenticationFailedException(genericFailureMessage);
        }

        var verifyResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
        if (verifyResult == PasswordVerificationResult.Failed)
        {
            await RegisterFailedLoginAsync(user, ct);
            throw new AuthenticationFailedException(genericFailureMessage);
        }

        user.FailedLoginCount = 0;
        user.LockoutUntilUtc = null;
        user.LastLoginAtUtc = DateTime.UtcNow;

        var result = IssueTokens(user);
        await _context.SaveChangesAsync(ct);
        return result;
    }

    public async Task<AuthResult> RefreshAsync(string refreshToken, CancellationToken ct)
    {
        const string genericFailureMessage = "Invalid or expired refresh token.";

        var tokenHash = JwtTokenService.HashToken(refreshToken);
        var existing = await _context.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.TokenHash == tokenHash, ct);

        if (existing is null || !existing.IsActive || !existing.User.IsActive)
        {
            throw new AuthenticationFailedException(genericFailureMessage);
        }

        existing.RevokedAtUtc = DateTime.UtcNow;

        var result = IssueTokens(existing.User);
        existing.ReplacedByTokenHash = JwtTokenService.HashToken(result.RefreshToken);

        await _context.SaveChangesAsync(ct);
        return result;
    }

    public async Task LogoutAsync(string refreshToken, CancellationToken ct)
    {
        var tokenHash = JwtTokenService.HashToken(refreshToken);
        var existing = await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.TokenHash == tokenHash, ct);

        if (existing is not null && existing.RevokedAtUtc is null)
        {
            existing.RevokedAtUtc = DateTime.UtcNow;
            await _context.SaveChangesAsync(ct);
        }
    }

    private async Task RegisterFailedLoginAsync(User user, CancellationToken ct)
    {
        var maxAttempts = _configuration.GetValue("Auth:MaxFailedLoginAttempts", 5);
        var lockoutMinutes = _configuration.GetValue("Auth:LockoutMinutes", 15);

        user.FailedLoginCount++;
        if (user.FailedLoginCount >= maxAttempts)
        {
            user.LockoutUntilUtc = DateTime.UtcNow.AddMinutes(lockoutMinutes);
            user.FailedLoginCount = 0;
        }

        await _context.SaveChangesAsync(ct);
    }

    private AuthResult IssueTokens(User user)
    {
        var (accessToken, accessExpiresAtUtc) = _jwtTokenService.GenerateAccessToken(user);
        var (refreshPlaintext, refreshHash, refreshExpiresAtUtc) = _jwtTokenService.GenerateRefreshToken();

        _context.RefreshTokens.Add(new RefreshToken
        {
            UserId = user.UserId,
            TokenHash = refreshHash,
            ExpiresAtUtc = refreshExpiresAtUtc,
            CreatedAtUtc = DateTime.UtcNow,
        });

        return new AuthResult
        {
            AccessToken = accessToken,
            AccessTokenExpiresAtUtc = accessExpiresAtUtc,
            RefreshToken = refreshPlaintext,
            RefreshTokenExpiresAtUtc = refreshExpiresAtUtc,
        };
    }
}

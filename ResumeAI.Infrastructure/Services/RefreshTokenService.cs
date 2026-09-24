using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using ResumeAI.Application.Interfaces;
using ResumeAI.Domain.Entities;
using ResumeAI.Infrastructure.Persistence;

namespace ResumeAI.Infrastructure.Services;

public class RefreshTokenService : IRefreshTokenService
{
    private readonly ApplicationDbContext _dbContext;

    public RefreshTokenService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<string> CreateRefreshTokenAsync(Guid userId)
    {
        var rawToken = Convert.ToBase64String(
            RandomNumberGenerator.GetBytes(64));

        var tokenHash = HashToken(rawToken);

        var refreshToken = new RefreshToken
        {
            UserId = userId,
            TokenHash = tokenHash,
            ExpiresOn = DateTime.UtcNow.AddDays(7),
            CreatedOn = DateTime.UtcNow
        };

        _dbContext.RefreshTokens.Add(refreshToken);

        await _dbContext.SaveChangesAsync();

        return rawToken;
    }

    public async Task<bool> ValidateRefreshTokenAsync(
        Guid userId,
        string refreshToken)
    {
        var tokenHash = HashToken(refreshToken);

        var token = await _dbContext.RefreshTokens
            .FirstOrDefaultAsync(x =>
                x.UserId == userId &&
                x.TokenHash == tokenHash);

        if (token is null)
        {
            return false;
        }

        if (token.RevokedOn.HasValue)
        {
            return false;
        }

        if (token.ExpiresOn <= DateTime.UtcNow)
        {
            return false;
        }

        return true;
    }

    public async Task RevokeRefreshTokenAsync(
        Guid userId,
        string refreshToken)
    {
        var tokenHash = HashToken(refreshToken);

        var token = await _dbContext.RefreshTokens
            .FirstOrDefaultAsync(x =>
                x.UserId == userId &&
                x.TokenHash == tokenHash);

        if (token is null)
        {
            return;
        }

        token.RevokedOn = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();
    }

    private static string HashToken(string token)
    {
        var hash = SHA256.HashData(
            Encoding.UTF8.GetBytes(token));

        return Convert.ToHexString(hash);
    }
}
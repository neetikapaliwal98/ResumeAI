namespace ResumeAI.Application.Interfaces;

public interface IRefreshTokenService
{
    Task<string> CreateRefreshTokenAsync(Guid userId);

    Task<bool> ValidateRefreshTokenAsync(
        Guid userId,
        string refreshToken);

    Task RevokeRefreshTokenAsync(
        Guid userId,
        string refreshToken);
}
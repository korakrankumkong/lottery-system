using LotterySystem.Application.DTOs;
using LotterySystem.Application.Interfaces;

namespace LotterySystem.Application.Services;

public sealed class AuthService(IAuthRepository authRepository, ITokenService tokenService, IUnitOfWork unitOfWork) : IAuthService
{
    public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default)
    {
        var user = await authRepository.GetByUsernameAsync(request.Username, cancellationToken);
        if (user is null) return null;

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash)) return null;

        var pair = tokenService.CreateTokenPair(user.Id, user.Username, user.FullName, user.Role.ToString());
        user.RefreshTokenHash = BCrypt.Net.BCrypt.HashPassword(pair.RefreshToken);
        user.RefreshTokenExpiresAtUtc = pair.RefreshTokenExpiresAtUtc;
        authRepository.Update(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return pair;
    }

    public async Task<LoginResponseDto?> RefreshAsync(RefreshTokenRequestDto request, CancellationToken cancellationToken = default)
    {
        var userId = tokenService.GetUserIdFromExpiredAccessToken(request.AccessToken);
        if (userId is null) return null;

        var user = await authRepository.GetByIdAsync(userId.Value, cancellationToken);
        if (user is null || user.RefreshTokenHash is null || user.RefreshTokenExpiresAtUtc is null) return null;
        if (user.RefreshTokenExpiresAtUtc.Value < DateTime.UtcNow) return null;
        if (!BCrypt.Net.BCrypt.Verify(request.RefreshToken, user.RefreshTokenHash)) return null;

        var pair = tokenService.CreateTokenPair(user.Id, user.Username, user.FullName, user.Role.ToString());
        user.RefreshTokenHash = BCrypt.Net.BCrypt.HashPassword(pair.RefreshToken);
        user.RefreshTokenExpiresAtUtc = pair.RefreshTokenExpiresAtUtc;
        authRepository.Update(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return pair;
    }

    public async Task LogoutAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await authRepository.GetByIdAsync(userId, cancellationToken);
        if (user is null) return;
        user.RefreshTokenHash = null;
        user.RefreshTokenExpiresAtUtc = null;
        authRepository.Update(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

namespace LotterySystem.Application.DTOs;

public sealed record LoginRequestDto(string Username, string Password);
public sealed record RefreshTokenRequestDto(string AccessToken, string RefreshToken);
public sealed record LoginResponseDto(string AccessToken, string RefreshToken, string FullName, string Role, DateTime AccessTokenExpiresAtUtc, DateTime RefreshTokenExpiresAtUtc);

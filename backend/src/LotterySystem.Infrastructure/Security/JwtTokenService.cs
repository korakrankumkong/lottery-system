using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using LotterySystem.Application.DTOs;
using LotterySystem.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace LotterySystem.Infrastructure.Security;

public sealed class JwtTokenService(IConfiguration configuration) : ITokenService
{
    public LoginResponseDto CreateTokenPair(Guid userId, string username, string fullName, string role)
    {
        var jwt = configuration.GetSection("Jwt");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var accessExpires = DateTime.UtcNow.AddMinutes(30);
        var refreshExpires = DateTime.UtcNow.AddDays(7);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, username),
            new Claim("fullName", fullName),
            new Claim(ClaimTypes.Role, role)
        };

        var token = new JwtSecurityToken(jwt["Issuer"], jwt["Audience"], claims, expires: accessExpires, signingCredentials: credentials);
        var accessToken = new JwtSecurityTokenHandler().WriteToken(token);
        var refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

        return new LoginResponseDto(accessToken, refreshToken, fullName, role, accessExpires, refreshExpires);
    }

    public Guid? GetUserIdFromExpiredAccessToken(string accessToken)
    {
        var jwt = configuration.GetSection("Jwt");
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = false,
            ValidIssuer = jwt["Issuer"],
            ValidAudience = jwt["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!))
        };

        var handler = new JwtSecurityTokenHandler();
        try
        {
            var principal = handler.ValidateToken(accessToken, tokenValidationParameters, out _);
            var sub = principal.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub || c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (Guid.TryParse(sub, out var userId)) return userId;
        }
        catch
        {
            return null;
        }

        return null;
    }
}

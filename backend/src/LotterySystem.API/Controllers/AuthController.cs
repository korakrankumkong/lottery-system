using System.IdentityModel.Tokens.Jwt;
using LotterySystem.Application.Common;
using LotterySystem.Application.DTOs;
using LotterySystem.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LotterySystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<LoginResponseDto>>> Login(LoginRequestDto request, CancellationToken cancellationToken)
    {
        var response = await authService.LoginAsync(request, cancellationToken);
        if (response is null)
        {
            return Unauthorized(new ApiResponse<LoginResponseDto>(false, "Invalid credentials", null));
        }

        return Ok(new ApiResponse<LoginResponseDto>(true, "Login successful", response));
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<LoginResponseDto>>> Refresh(RefreshTokenRequestDto request, CancellationToken cancellationToken)
    {
        var response = await authService.RefreshAsync(request, cancellationToken);
        if (response is null)
        {
            return Unauthorized(new ApiResponse<LoginResponseDto>(false, "Invalid refresh token", null));
        }

        return Ok(new ApiResponse<LoginResponseDto>(true, "Token refreshed", response));
    }

    [HttpPost("logout")]
    [Authorize(Policy = "StaffOrAdmin")]
    public async Task<ActionResult<ApiResponse<object>>> Logout(CancellationToken cancellationToken)
    {
        var sub = User.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub || c.Type == "sub")?.Value;
        if (sub is null || !Guid.TryParse(sub, out var userId))
        {
            return Unauthorized(new ApiResponse<object>(false, "Invalid token subject", null));
        }

        await authService.LogoutAsync(userId, cancellationToken);
        return Ok(new ApiResponse<object>(true, "Logged out", null));
    }
}

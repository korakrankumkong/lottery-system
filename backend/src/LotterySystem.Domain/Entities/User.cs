using LotterySystem.Domain.Common;
using LotterySystem.Domain.Enums;

namespace LotterySystem.Domain.Entities;

public sealed class User : BaseEntity
{
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.Staff;
    public string? RefreshTokenHash { get; set; }
    public DateTime? RefreshTokenExpiresAtUtc { get; set; }
}

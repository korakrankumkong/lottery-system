using LotterySystem.Domain.Common;

namespace LotterySystem.Domain.Entities;

public sealed class AuditLog : BaseEntity
{
    public Guid? UserId { get; set; }
    public User? User { get; set; }
    public string Action { get; set; } = string.Empty;
    public string EntityName { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
    public string? ChangesJson { get; set; }
    public string? IpAddress { get; set; }
}

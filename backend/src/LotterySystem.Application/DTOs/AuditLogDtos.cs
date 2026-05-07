namespace LotterySystem.Application.DTOs;

public sealed record AuditLogDto(Guid Id, Guid? UserId, string Action, string EntityName, string EntityId, DateTime CreatedAtUtc, string? IpAddress);

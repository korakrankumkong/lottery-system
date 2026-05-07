namespace LotterySystem.Application.DTOs;

public sealed record LotteryRoundDto(Guid Id, string Code, DateOnly DrawDate, bool IsClosed);
public sealed record UpsertLotteryRoundDto(string Code, DateOnly DrawDate, bool IsClosed);

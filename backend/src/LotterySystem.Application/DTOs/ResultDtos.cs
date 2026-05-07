namespace LotterySystem.Application.DTOs;

public sealed record LotteryResultDto(Guid Id, Guid LotteryRoundId, string PrizeType, string WinningNumber);
public sealed record UpsertLotteryResultDto(Guid LotteryRoundId, string PrizeType, string WinningNumber);
public sealed record TicketCheckResultDto(string Number, bool IsWinner, string? PrizeType);

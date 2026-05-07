namespace LotterySystem.Application.DTOs;

public sealed record TicketItemRequestDto(string Number, decimal Amount);
public sealed record CreateTicketRequestDto(Guid CustomerId, Guid LotteryRoundId, List<TicketItemRequestDto> Items);
public sealed record UpdateTicketRequestDto(Guid CustomerId, Guid LotteryRoundId, List<TicketItemRequestDto> Items);
public sealed record TicketDto(Guid Id, Guid CustomerId, Guid LotteryRoundId, decimal TotalAmount, DateTime CreatedAtUtc, List<TicketItemDto> Items);
public sealed record TicketItemDto(Guid Id, string Number, decimal Amount);
public sealed record TicketHistoryQueryDto(Guid? CustomerId, Guid? LotteryRoundId, DateTime? FromUtc, DateTime? ToUtc, int Page = 1, int PageSize = 50);
public sealed record TicketSearchQueryDto(string? Keyword, DateTime? FromUtc, DateTime? ToUtc);
public sealed record DailySummaryDto(DateOnly Date, int TicketCount, decimal TotalAmount);
public sealed record RiskyNumberDto(string Number, int Frequency, decimal TotalAmount);
public sealed record TotalsDto(int TicketCount, int ItemCount, decimal TotalAmount);

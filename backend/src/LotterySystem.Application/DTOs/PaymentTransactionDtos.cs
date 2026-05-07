namespace LotterySystem.Application.DTOs;

public sealed record PaymentTransactionDto(Guid Id, Guid TicketId, decimal Amount, string Method, string Status, string? ReferenceNo, DateTime PaidAtUtc);
public sealed record CreatePaymentTransactionDto(Guid TicketId, decimal Amount, string Method, string Status, string? ReferenceNo, DateTime PaidAtUtc);

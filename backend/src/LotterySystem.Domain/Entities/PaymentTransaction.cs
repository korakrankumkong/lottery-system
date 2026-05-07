using LotterySystem.Domain.Common;

namespace LotterySystem.Domain.Entities;

public sealed class PaymentTransaction : BaseEntity
{
    public Guid TicketId { get; set; }
    public Ticket? Ticket { get; set; }
    public decimal Amount { get; set; }
    public string Method { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? ReferenceNo { get; set; }
    public DateTime PaidAtUtc { get; set; }
}

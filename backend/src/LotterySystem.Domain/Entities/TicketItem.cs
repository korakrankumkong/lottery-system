using LotterySystem.Domain.Common;

namespace LotterySystem.Domain.Entities;

public sealed class TicketItem : BaseEntity
{
    public Guid TicketId { get; set; }
    public Ticket? Ticket { get; set; }
    public string Number { get; set; } = string.Empty;
    public decimal Amount { get; set; }
}

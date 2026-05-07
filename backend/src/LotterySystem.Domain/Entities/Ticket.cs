using LotterySystem.Domain.Common;

namespace LotterySystem.Domain.Entities;

public sealed class Ticket : BaseEntity
{
    public Guid CustomerId { get; set; }
    public Customer? Customer { get; set; }
    public Guid LotteryRoundId { get; set; }
    public LotteryRound? LotteryRound { get; set; }
    public decimal TotalAmount { get; set; }
    public ICollection<TicketItem> Items { get; set; } = new List<TicketItem>();
    public ICollection<PaymentTransaction> PaymentTransactions { get; set; } = new List<PaymentTransaction>();
}

using LotterySystem.Domain.Common;

namespace LotterySystem.Domain.Entities;

public sealed class LotteryRound : BaseEntity
{
    public string Code { get; set; } = string.Empty;
    public DateOnly DrawDate { get; set; }
    public bool IsClosed { get; set; }
    public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    public ICollection<LotteryResult> Results { get; set; } = new List<LotteryResult>();
}

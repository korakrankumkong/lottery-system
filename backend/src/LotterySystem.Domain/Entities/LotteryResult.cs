using LotterySystem.Domain.Common;

namespace LotterySystem.Domain.Entities;

public sealed class LotteryResult : BaseEntity
{
    public Guid LotteryRoundId { get; set; }
    public LotteryRound? LotteryRound { get; set; }
    public string PrizeType { get; set; } = string.Empty;
    public string WinningNumber { get; set; } = string.Empty;
}

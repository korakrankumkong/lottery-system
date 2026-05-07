using LotterySystem.Domain.Common;

namespace LotterySystem.Domain.Entities;

public sealed class Customer : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}

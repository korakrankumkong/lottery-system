using LotterySystem.Application.Interfaces;
using LotterySystem.Domain.Common;
using LotterySystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LotterySystem.Infrastructure.Persistence;

public class Repository<T>(LotteryDbContext dbContext) : IRepository<T> where T : BaseEntity
{
    protected readonly LotteryDbContext DbContext = dbContext;

    public Task<List<T>> GetAllAsync(CancellationToken cancellationToken = default) => DbContext.Set<T>().ToListAsync(cancellationToken);
    public Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) => DbContext.Set<T>().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    public Task AddAsync(T entity, CancellationToken cancellationToken = default) => DbContext.Set<T>().AddAsync(entity, cancellationToken).AsTask();
    public void Update(T entity) => DbContext.Set<T>().Update(entity);
    public void Remove(T entity) => DbContext.Set<T>().Remove(entity);
}

public sealed class AuthRepository(LotteryDbContext dbContext) : IAuthRepository
{
    public Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default) =>
        dbContext.Users.FirstOrDefaultAsync(x => x.Username == username, cancellationToken);

    public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        dbContext.Users.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public void Update(User user) => dbContext.Users.Update(user);
}

public sealed class TicketRepository(LotteryDbContext dbContext) : Repository<Ticket>(dbContext), ITicketRepository
{
    public Task<List<Ticket>> GetByRoundAsync(Guid roundId, CancellationToken cancellationToken = default) =>
        DbContext.Tickets.Include(x => x.Items).Where(x => x.LotteryRoundId == roundId).ToListAsync(cancellationToken);

    public Task<List<Ticket>> SearchAsync(string? keyword, DateTime? fromUtc, DateTime? toUtc, CancellationToken cancellationToken = default)
    {
        var query = DbContext.Tickets
            .Include(t => t.Items)
            .Include(t => t.Customer)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(t => t.Items.Any(i => i.Number.Contains(keyword)) || (t.Customer != null && t.Customer.Name.Contains(keyword)));
        }

        if (fromUtc.HasValue) query = query.Where(t => t.CreatedAtUtc >= fromUtc.Value);
        if (toUtc.HasValue) query = query.Where(t => t.CreatedAtUtc <= toUtc.Value);

        return query.OrderByDescending(t => t.CreatedAtUtc).Take(500).ToListAsync(cancellationToken);
    }

    public Task<List<Ticket>> GetHistoryAsync(Guid? customerId, Guid? roundId, DateTime? fromUtc, DateTime? toUtc, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = DbContext.Tickets.Include(x => x.Items).AsQueryable();
        if (customerId.HasValue) query = query.Where(x => x.CustomerId == customerId.Value);
        if (roundId.HasValue) query = query.Where(x => x.LotteryRoundId == roundId.Value);
        if (fromUtc.HasValue) query = query.Where(x => x.CreatedAtUtc >= fromUtc.Value);
        if (toUtc.HasValue) query = query.Where(x => x.CreatedAtUtc <= toUtc.Value);

        var skip = (Math.Max(page, 1) - 1) * Math.Clamp(pageSize, 1, 200);
        return query.OrderByDescending(x => x.CreatedAtUtc).Skip(skip).Take(Math.Clamp(pageSize, 1, 200)).ToListAsync(cancellationToken);
    }

    public async Task<List<(DateOnly Date, int TicketCount, decimal TotalAmount)>> GetDailySummaryAsync(DateTime fromUtc, DateTime toUtc, CancellationToken cancellationToken = default)
    {
        var rows = await DbContext.Tickets
            .Where(x => x.CreatedAtUtc >= fromUtc && x.CreatedAtUtc <= toUtc)
            .GroupBy(x => DateOnly.FromDateTime(x.CreatedAtUtc))
            .Select(g => new { Date = g.Key, TicketCount = g.Count(), TotalAmount = g.Sum(x => x.TotalAmount) })
            .OrderBy(x => x.Date)
            .ToListAsync(cancellationToken);

        return rows.Select(x => (x.Date, x.TicketCount, x.TotalAmount)).ToList();
    }

    public async Task<List<(string Number, int Frequency, decimal TotalAmount)>> GetRiskyNumbersAsync(DateTime fromUtc, DateTime toUtc, int top, CancellationToken cancellationToken = default)
    {
        var rows = await DbContext.TicketItems
            .Where(i => i.Ticket != null && i.Ticket.CreatedAtUtc >= fromUtc && i.Ticket.CreatedAtUtc <= toUtc)
            .GroupBy(i => i.Number)
            .Select(g => new { Number = g.Key, Frequency = g.Count(), TotalAmount = g.Sum(x => x.Amount) })
            .OrderByDescending(x => x.Frequency)
            .ThenByDescending(x => x.TotalAmount)
            .Take(top)
            .ToListAsync(cancellationToken);

        return rows.Select(x => (x.Number, x.Frequency, x.TotalAmount)).ToList();
    }

    public async Task<(int TicketCount, int ItemCount, decimal TotalAmount)> GetTotalsAsync(DateTime? fromUtc, DateTime? toUtc, CancellationToken cancellationToken = default)
    {
        var ticketQuery = DbContext.Tickets.AsQueryable();
        var itemQuery = DbContext.TicketItems.AsQueryable();

        if (fromUtc.HasValue)
        {
            ticketQuery = ticketQuery.Where(x => x.CreatedAtUtc >= fromUtc.Value);
            itemQuery = itemQuery.Where(x => x.Ticket != null && x.Ticket.CreatedAtUtc >= fromUtc.Value);
        }
        if (toUtc.HasValue)
        {
            ticketQuery = ticketQuery.Where(x => x.CreatedAtUtc <= toUtc.Value);
            itemQuery = itemQuery.Where(x => x.Ticket != null && x.Ticket.CreatedAtUtc <= toUtc.Value);
        }

        var ticketCount = await ticketQuery.CountAsync(cancellationToken);
        var itemCount = await itemQuery.CountAsync(cancellationToken);
        var totalAmount = await ticketQuery.Select(x => x.TotalAmount).DefaultIfEmpty(0).SumAsync(cancellationToken);
        return (ticketCount, itemCount, totalAmount);
    }
}


public sealed class ReportRepository(LotteryDbContext dbContext) : IReportRepository
{
    public async Task<(List<(DateOnly Date, int TicketCount, decimal SalesAmount)> Rows, int TotalCount)> GetDailySalesAsync(DateTime fromUtc, DateTime toUtc, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var grouped = dbContext.Tickets
            .Where(t => t.CreatedAtUtc >= fromUtc && t.CreatedAtUtc <= toUtc)
            .GroupBy(t => DateOnly.FromDateTime(t.CreatedAtUtc))
            .Select(g => new { Date = g.Key, TicketCount = g.Count(), SalesAmount = g.Sum(x => x.TotalAmount) });

        var totalCount = await grouped.CountAsync(cancellationToken);
        var rows = await grouped.OrderByDescending(x => x.Date)
            .Skip((Math.Max(page, 1) - 1) * Math.Clamp(pageSize, 1, 200))
            .Take(Math.Clamp(pageSize, 1, 200))
            .ToListAsync(cancellationToken);

        return (rows.Select(x => (x.Date, x.TicketCount, x.SalesAmount)).ToList(), totalCount);
    }

    public async Task<(List<(string Number, int Frequency, decimal TotalAmount)> Rows, int TotalCount)> GetNumberSummaryAsync(DateTime fromUtc, DateTime toUtc, string? keyword, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var grouped = dbContext.TicketItems
            .Where(i => i.Ticket != null && i.Ticket.CreatedAtUtc >= fromUtc && i.Ticket.CreatedAtUtc <= toUtc)
            .GroupBy(i => i.Number)
            .Select(g => new { Number = g.Key, Frequency = g.Count(), TotalAmount = g.Sum(x => x.Amount) });

        if (!string.IsNullOrWhiteSpace(keyword)) grouped = grouped.Where(x => x.Number.Contains(keyword));

        var totalCount = await grouped.CountAsync(cancellationToken);
        var rows = await grouped.OrderByDescending(x => x.Frequency)
            .Skip((Math.Max(page, 1) - 1) * Math.Clamp(pageSize, 1, 200))
            .Take(Math.Clamp(pageSize, 1, 200))
            .ToListAsync(cancellationToken);

        return (rows.Select(x => (x.Number, x.Frequency, x.TotalAmount)).ToList(), totalCount);
    }

    public async Task<(List<(string Number, int Frequency, decimal TotalAmount, decimal RiskScore)> Rows, int TotalCount)> GetRiskNumbersAsync(DateTime fromUtc, DateTime toUtc, string? keyword, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var grouped = dbContext.TicketItems
            .Where(i => i.Ticket != null && i.Ticket.CreatedAtUtc >= fromUtc && i.Ticket.CreatedAtUtc <= toUtc)
            .GroupBy(i => i.Number)
            .Select(g => new { Number = g.Key, Frequency = g.Count(), TotalAmount = g.Sum(x => x.Amount), RiskScore = g.Count() * g.Sum(x => x.Amount) / 100m });

        if (!string.IsNullOrWhiteSpace(keyword)) grouped = grouped.Where(x => x.Number.Contains(keyword));

        var totalCount = await grouped.CountAsync(cancellationToken);
        var rows = await grouped.OrderByDescending(x => x.RiskScore)
            .Skip((Math.Max(page, 1) - 1) * Math.Clamp(pageSize, 1, 200))
            .Take(Math.Clamp(pageSize, 1, 200))
            .ToListAsync(cancellationToken);

        return (rows.Select(x => (x.Number, x.Frequency, x.TotalAmount, x.RiskScore)).ToList(), totalCount);
    }

    public async Task<(List<(Guid CustomerId, string CustomerName, int TicketCount, decimal TotalAmount, DateTime LastTicketAtUtc)> Rows, int TotalCount)> GetCustomerHistoryAsync(DateTime fromUtc, DateTime toUtc, string? keyword, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var grouped = dbContext.Tickets
            .Where(t => t.CreatedAtUtc >= fromUtc && t.CreatedAtUtc <= toUtc && t.Customer != null)
            .GroupBy(t => new { t.CustomerId, t.Customer!.Name })
            .Select(g => new { g.Key.CustomerId, CustomerName = g.Key.Name, TicketCount = g.Count(), TotalAmount = g.Sum(x => x.TotalAmount), LastTicketAtUtc = g.Max(x => x.CreatedAtUtc) });

        if (!string.IsNullOrWhiteSpace(keyword)) grouped = grouped.Where(x => x.CustomerName.Contains(keyword));

        var totalCount = await grouped.CountAsync(cancellationToken);
        var rows = await grouped.OrderByDescending(x => x.LastTicketAtUtc)
            .Skip((Math.Max(page, 1) - 1) * Math.Clamp(pageSize, 1, 200))
            .Take(Math.Clamp(pageSize, 1, 200))
            .ToListAsync(cancellationToken);

        return (rows.Select(x => (x.CustomerId, x.CustomerName, x.TicketCount, x.TotalAmount, x.LastTicketAtUtc)).ToList(), totalCount);
    }

    public async Task<(List<(DateOnly Date, decimal SalesAmount, decimal PayoutAmount, decimal ProfitAmount)> Rows, int TotalCount)> GetProfitLossAsync(DateTime fromUtc, DateTime toUtc, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var grouped = dbContext.Tickets
            .Where(t => t.CreatedAtUtc >= fromUtc && t.CreatedAtUtc <= toUtc)
            .GroupBy(t => DateOnly.FromDateTime(t.CreatedAtUtc))
            .Select(g => new { Date = g.Key, SalesAmount = g.Sum(x => x.TotalAmount) });

        var totalCount = await grouped.CountAsync(cancellationToken);
        var rows = await grouped.OrderByDescending(x => x.Date)
            .Skip((Math.Max(page, 1) - 1) * Math.Clamp(pageSize, 1, 200))
            .Take(Math.Clamp(pageSize, 1, 200))
            .ToListAsync(cancellationToken);

        return (rows.Select(x => (x.Date, x.SalesAmount, x.SalesAmount * 0.62m, x.SalesAmount * 0.38m)).ToList(), totalCount);
    }
}
public sealed class UnitOfWork(LotteryDbContext dbContext) : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) => dbContext.SaveChangesAsync(cancellationToken);
}



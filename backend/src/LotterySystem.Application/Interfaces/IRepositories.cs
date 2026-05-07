using LotterySystem.Domain.Common;
using LotterySystem.Domain.Entities;

namespace LotterySystem.Application.Interfaces;

public interface IRepository<T> where T : BaseEntity
{
    Task<List<T>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(T entity, CancellationToken cancellationToken = default);
    void Update(T entity);
    void Remove(T entity);
}

public interface IAuthRepository
{
    Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    void Update(User user);
}

public interface ITicketRepository : IRepository<Ticket>
{
    Task<List<Ticket>> GetByRoundAsync(Guid roundId, CancellationToken cancellationToken = default);
    Task<List<Ticket>> SearchAsync(string? keyword, DateTime? fromUtc, DateTime? toUtc, CancellationToken cancellationToken = default);
    Task<List<Ticket>> GetHistoryAsync(Guid? customerId, Guid? roundId, DateTime? fromUtc, DateTime? toUtc, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<List<(DateOnly Date, int TicketCount, decimal TotalAmount)>> GetDailySummaryAsync(DateTime fromUtc, DateTime toUtc, CancellationToken cancellationToken = default);
    Task<List<(string Number, int Frequency, decimal TotalAmount)>> GetRiskyNumbersAsync(DateTime fromUtc, DateTime toUtc, int top, CancellationToken cancellationToken = default);
    Task<(int TicketCount, int ItemCount, decimal TotalAmount)> GetTotalsAsync(DateTime? fromUtc, DateTime? toUtc, CancellationToken cancellationToken = default);
}

public interface IReportRepository
{
    Task<(List<(DateOnly Date, int TicketCount, decimal SalesAmount)> Rows, int TotalCount)> GetDailySalesAsync(DateTime fromUtc, DateTime toUtc, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<(List<(string Number, int Frequency, decimal TotalAmount)> Rows, int TotalCount)> GetNumberSummaryAsync(DateTime fromUtc, DateTime toUtc, string? keyword, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<(List<(string Number, int Frequency, decimal TotalAmount, decimal RiskScore)> Rows, int TotalCount)> GetRiskNumbersAsync(DateTime fromUtc, DateTime toUtc, string? keyword, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<(List<(Guid CustomerId, string CustomerName, int TicketCount, decimal TotalAmount, DateTime LastTicketAtUtc)> Rows, int TotalCount)> GetCustomerHistoryAsync(DateTime fromUtc, DateTime toUtc, string? keyword, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<(List<(DateOnly Date, decimal SalesAmount, decimal PayoutAmount, decimal ProfitAmount)> Rows, int TotalCount)> GetProfitLossAsync(DateTime fromUtc, DateTime toUtc, int page, int pageSize, CancellationToken cancellationToken = default);
}

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

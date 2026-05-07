namespace LotterySystem.Application.DTOs;

public sealed record DashboardReportDto(int TotalCustomers, int TotalTickets, decimal TotalSales, int OpenRounds);

public sealed record ReportFilterDto(DateTime FromUtc, DateTime ToUtc, string? Keyword = null, int Page = 1, int PageSize = 20);

public sealed record DailySalesRowDto(DateOnly Date, int TicketCount, decimal SalesAmount);
public sealed record NumberSummaryRowDto(string Number, int Frequency, decimal TotalAmount);
public sealed record RiskNumberRowDto(string Number, int Frequency, decimal TotalAmount, decimal RiskScore);
public sealed record CustomerHistoryRowDto(Guid CustomerId, string CustomerName, int TicketCount, decimal TotalAmount, DateTime LastTicketAtUtc);
public sealed record ProfitLossRowDto(DateOnly Date, decimal SalesAmount, decimal PayoutAmount, decimal ProfitAmount);

public sealed record PagedResultDto<T>(IReadOnlyList<T> Items, int Page, int PageSize, int TotalCount);

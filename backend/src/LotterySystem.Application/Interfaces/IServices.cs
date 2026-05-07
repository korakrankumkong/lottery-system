using LotterySystem.Application.DTOs;

namespace LotterySystem.Application.Interfaces;

public interface ITokenService
{
    LoginResponseDto CreateTokenPair(Guid userId, string username, string fullName, string role);
    Guid? GetUserIdFromExpiredAccessToken(string accessToken);
}

public interface IAuthService
{
    Task<LoginResponseDto?> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default);
    Task<LoginResponseDto?> RefreshAsync(RefreshTokenRequestDto request, CancellationToken cancellationToken = default);
    Task LogoutAsync(Guid userId, CancellationToken cancellationToken = default);
}

public interface ITicketService
{
    Task<TicketDto> CreateAsync(CreateTicketRequestDto request, CancellationToken cancellationToken = default);
    Task<TicketDto> UpdateAsync(Guid id, UpdateTicketRequestDto request, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<TicketDto>> GetHistoryAsync(TicketHistoryQueryDto query, CancellationToken cancellationToken = default);
    Task<List<TicketDto>> SearchAsync(TicketSearchQueryDto query, CancellationToken cancellationToken = default);
    Task<List<DailySummaryDto>> GetDailySummaryAsync(DateTime fromUtc, DateTime toUtc, CancellationToken cancellationToken = default);
    Task<List<RiskyNumberDto>> GetRiskyNumbersAsync(DateTime fromUtc, DateTime toUtc, int top = 20, CancellationToken cancellationToken = default);
    Task<TotalsDto> GetTotalsAsync(DateTime? fromUtc, DateTime? toUtc, CancellationToken cancellationToken = default);
}

public interface IReportService
{
    Task<PagedResultDto<DailySalesRowDto>> GetDailySalesAsync(ReportFilterDto filter, CancellationToken cancellationToken = default);
    Task<PagedResultDto<NumberSummaryRowDto>> GetNumberSummaryAsync(ReportFilterDto filter, CancellationToken cancellationToken = default);
    Task<PagedResultDto<RiskNumberRowDto>> GetRiskNumbersAsync(ReportFilterDto filter, CancellationToken cancellationToken = default);
    Task<PagedResultDto<CustomerHistoryRowDto>> GetCustomerHistoryAsync(ReportFilterDto filter, CancellationToken cancellationToken = default);
    Task<PagedResultDto<ProfitLossRowDto>> GetProfitLossAsync(ReportFilterDto filter, CancellationToken cancellationToken = default);
    Task<byte[]> ExportDailySalesExcelAsync(ReportFilterDto filter, CancellationToken cancellationToken = default);
    Task<byte[]> ExportDailySalesPdfAsync(ReportFilterDto filter, CancellationToken cancellationToken = default);
}

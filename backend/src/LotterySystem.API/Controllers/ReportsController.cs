using LotterySystem.Application.Common;
using LotterySystem.Application.DTOs;
using LotterySystem.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LotterySystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "AdminOnly")]
public sealed class ReportsController(IReportService reportService) : ControllerBase
{
    [HttpGet("daily-sales")]
    public async Task<ActionResult<ApiResponse<PagedResultDto<DailySalesRowDto>>>> DailySales([FromQuery] ReportFilterDto filter, CancellationToken ct)
        => Ok(new ApiResponse<PagedResultDto<DailySalesRowDto>>(true, "Daily sales", await reportService.GetDailySalesAsync(filter, ct)));

    [HttpGet("number-summary")]
    public async Task<ActionResult<ApiResponse<PagedResultDto<NumberSummaryRowDto>>>> NumberSummary([FromQuery] ReportFilterDto filter, CancellationToken ct)
        => Ok(new ApiResponse<PagedResultDto<NumberSummaryRowDto>>(true, "Number summary", await reportService.GetNumberSummaryAsync(filter, ct)));

    [HttpGet("risk-numbers")]
    public async Task<ActionResult<ApiResponse<PagedResultDto<RiskNumberRowDto>>>> RiskNumbers([FromQuery] ReportFilterDto filter, CancellationToken ct)
        => Ok(new ApiResponse<PagedResultDto<RiskNumberRowDto>>(true, "Risk numbers", await reportService.GetRiskNumbersAsync(filter, ct)));

    [HttpGet("customer-history")]
    public async Task<ActionResult<ApiResponse<PagedResultDto<CustomerHistoryRowDto>>>> CustomerHistory([FromQuery] ReportFilterDto filter, CancellationToken ct)
        => Ok(new ApiResponse<PagedResultDto<CustomerHistoryRowDto>>(true, "Customer history", await reportService.GetCustomerHistoryAsync(filter, ct)));

    [HttpGet("profit-loss")]
    public async Task<ActionResult<ApiResponse<PagedResultDto<ProfitLossRowDto>>>> ProfitLoss([FromQuery] ReportFilterDto filter, CancellationToken ct)
        => Ok(new ApiResponse<PagedResultDto<ProfitLossRowDto>>(true, "Profit loss", await reportService.GetProfitLossAsync(filter, ct)));

    [HttpGet("daily-sales/export/excel")]
    public async Task<IActionResult> ExportDailySalesExcel([FromQuery] ReportFilterDto filter, CancellationToken ct)
    {
        var file = await reportService.ExportDailySalesExcelAsync(filter, ct);
        return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"daily-sales-{DateTime.UtcNow:yyyyMMddHHmmss}.xlsx");
    }

    [HttpGet("daily-sales/export/pdf")]
    public async Task<IActionResult> ExportDailySalesPdf([FromQuery] ReportFilterDto filter, CancellationToken ct)
    {
        var file = await reportService.ExportDailySalesPdfAsync(filter, ct);
        return File(file, "application/pdf", $"daily-sales-{DateTime.UtcNow:yyyyMMddHHmmss}.pdf");
    }
}

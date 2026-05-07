using ClosedXML.Excel;
using LotterySystem.Application.DTOs;
using LotterySystem.Application.Interfaces;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace LotterySystem.Application.Services;

public sealed class ReportService(IReportRepository reportRepository) : IReportService
{
    public async Task<PagedResultDto<DailySalesRowDto>> GetDailySalesAsync(ReportFilterDto filter, CancellationToken cancellationToken = default)
    {
        var (rows, totalCount) = await reportRepository.GetDailySalesAsync(filter.FromUtc, filter.ToUtc, filter.Page, filter.PageSize, cancellationToken);
        return new PagedResultDto<DailySalesRowDto>(rows.Select(x => new DailySalesRowDto(x.Date, x.TicketCount, x.SalesAmount)).ToList(), filter.Page, filter.PageSize, totalCount);
    }

    public async Task<PagedResultDto<NumberSummaryRowDto>> GetNumberSummaryAsync(ReportFilterDto filter, CancellationToken cancellationToken = default)
    {
        var (rows, totalCount) = await reportRepository.GetNumberSummaryAsync(filter.FromUtc, filter.ToUtc, filter.Keyword, filter.Page, filter.PageSize, cancellationToken);
        return new PagedResultDto<NumberSummaryRowDto>(rows.Select(x => new NumberSummaryRowDto(x.Number, x.Frequency, x.TotalAmount)).ToList(), filter.Page, filter.PageSize, totalCount);
    }

    public async Task<PagedResultDto<RiskNumberRowDto>> GetRiskNumbersAsync(ReportFilterDto filter, CancellationToken cancellationToken = default)
    {
        var (rows, totalCount) = await reportRepository.GetRiskNumbersAsync(filter.FromUtc, filter.ToUtc, filter.Keyword, filter.Page, filter.PageSize, cancellationToken);
        return new PagedResultDto<RiskNumberRowDto>(rows.Select(x => new RiskNumberRowDto(x.Number, x.Frequency, x.TotalAmount, x.RiskScore)).ToList(), filter.Page, filter.PageSize, totalCount);
    }

    public async Task<PagedResultDto<CustomerHistoryRowDto>> GetCustomerHistoryAsync(ReportFilterDto filter, CancellationToken cancellationToken = default)
    {
        var (rows, totalCount) = await reportRepository.GetCustomerHistoryAsync(filter.FromUtc, filter.ToUtc, filter.Keyword, filter.Page, filter.PageSize, cancellationToken);
        return new PagedResultDto<CustomerHistoryRowDto>(rows.Select(x => new CustomerHistoryRowDto(x.CustomerId, x.CustomerName, x.TicketCount, x.TotalAmount, x.LastTicketAtUtc)).ToList(), filter.Page, filter.PageSize, totalCount);
    }

    public async Task<PagedResultDto<ProfitLossRowDto>> GetProfitLossAsync(ReportFilterDto filter, CancellationToken cancellationToken = default)
    {
        var (rows, totalCount) = await reportRepository.GetProfitLossAsync(filter.FromUtc, filter.ToUtc, filter.Page, filter.PageSize, cancellationToken);
        return new PagedResultDto<ProfitLossRowDto>(rows.Select(x => new ProfitLossRowDto(x.Date, x.SalesAmount, x.PayoutAmount, x.ProfitAmount)).ToList(), filter.Page, filter.PageSize, totalCount);
    }

    public async Task<byte[]> ExportDailySalesExcelAsync(ReportFilterDto filter, CancellationToken cancellationToken = default)
    {
        var report = await GetDailySalesAsync(filter with { Page = 1, PageSize = 5000 }, cancellationToken);
        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("DailySales");
        ws.Cell(1, 1).Value = "Date";
        ws.Cell(1, 2).Value = "Tickets";
        ws.Cell(1, 3).Value = "Sales";

        for (var i = 0; i < report.Items.Count; i++)
        {
            ws.Cell(i + 2, 1).Value = report.Items[i].Date.ToString("yyyy-MM-dd");
            ws.Cell(i + 2, 2).Value = report.Items[i].TicketCount;
            ws.Cell(i + 2, 3).Value = report.Items[i].SalesAmount;
        }

        ws.Columns().AdjustToContents();
        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    public async Task<byte[]> ExportDailySalesPdfAsync(ReportFilterDto filter, CancellationToken cancellationToken = default)
    {
        QuestPDF.Settings.License = LicenseType.Community;
        var report = await GetDailySalesAsync(filter with { Page = 1, PageSize = 5000 }, cancellationToken);

        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(30);
                page.Content().Column(col =>
                {
                    col.Item().Text("Daily Sales Report").FontSize(18).Bold();
                    col.Item().Text($"Range: {filter.FromUtc:yyyy-MM-dd} - {filter.ToUtc:yyyy-MM-dd}");
                    col.Item().PaddingTop(10).Table(table =>
                    {
                        table.ColumnsDefinition(c =>
                        {
                            c.RelativeColumn();
                            c.RelativeColumn();
                            c.RelativeColumn();
                        });

                        table.Header(h =>
                        {
                            h.Cell().Text("Date").Bold();
                            h.Cell().Text("Tickets").Bold();
                            h.Cell().Text("Sales").Bold();
                        });

                        foreach (var row in report.Items)
                        {
                            table.Cell().Text(row.Date.ToString("yyyy-MM-dd"));
                            table.Cell().Text(row.TicketCount.ToString());
                            table.Cell().Text(row.SalesAmount.ToString("N2"));
                        }
                    });
                });
            });
        }).GeneratePdf();
    }
}

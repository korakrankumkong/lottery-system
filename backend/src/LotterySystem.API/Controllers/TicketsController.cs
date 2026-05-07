using LotterySystem.Application.Common;
using LotterySystem.Application.DTOs;
using LotterySystem.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LotterySystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "StaffOrAdmin")]
public sealed class TicketsController(ITicketService ticketService) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<ApiResponse<TicketDto>>> Create([FromBody] CreateTicketRequestDto request, CancellationToken cancellationToken)
    {
        var result = await ticketService.CreateAsync(request, cancellationToken);
        return Ok(new ApiResponse<TicketDto>(true, "Ticket created", result));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse<TicketDto>>> Update(Guid id, [FromBody] UpdateTicketRequestDto request, CancellationToken cancellationToken)
    {
        var result = await ticketService.UpdateAsync(id, request, cancellationToken);
        return Ok(new ApiResponse<TicketDto>(true, "Ticket updated", result));
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(Guid id, CancellationToken cancellationToken)
    {
        await ticketService.DeleteAsync(id, cancellationToken);
        return Ok(new ApiResponse<object>(true, "Ticket deleted", null));
    }

    [HttpGet("history")]
    public async Task<ActionResult<ApiResponse<List<TicketDto>>>> History([FromQuery] TicketHistoryQueryDto query, CancellationToken cancellationToken)
    {
        var result = await ticketService.GetHistoryAsync(query, cancellationToken);
        return Ok(new ApiResponse<List<TicketDto>>(true, "Ticket history", result));
    }

    [HttpGet("search")]
    public async Task<ActionResult<ApiResponse<List<TicketDto>>>> Search([FromQuery] TicketSearchQueryDto query, CancellationToken cancellationToken)
    {
        var result = await ticketService.SearchAsync(query, cancellationToken);
        return Ok(new ApiResponse<List<TicketDto>>(true, "Search results", result));
    }

    [HttpGet("daily-summary")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<ApiResponse<List<DailySummaryDto>>>> DailySummary([FromQuery] DateTime fromUtc, [FromQuery] DateTime toUtc, CancellationToken cancellationToken)
    {
        var result = await ticketService.GetDailySummaryAsync(fromUtc, toUtc, cancellationToken);
        return Ok(new ApiResponse<List<DailySummaryDto>>(true, "Daily summary", result));
    }

    [HttpGet("risky-numbers")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<ApiResponse<List<RiskyNumberDto>>>> RiskyNumbers([FromQuery] DateTime fromUtc, [FromQuery] DateTime toUtc, [FromQuery] int top = 20, CancellationToken cancellationToken = default)
    {
        var result = await ticketService.GetRiskyNumbersAsync(fromUtc, toUtc, top, cancellationToken);
        return Ok(new ApiResponse<List<RiskyNumberDto>>(true, "Risky numbers", result));
    }

    [HttpGet("totals")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<ApiResponse<TotalsDto>>> Totals([FromQuery] DateTime? fromUtc, [FromQuery] DateTime? toUtc, CancellationToken cancellationToken)
    {
        var result = await ticketService.GetTotalsAsync(fromUtc, toUtc, cancellationToken);
        return Ok(new ApiResponse<TotalsDto>(true, "Totals", result));
    }
}

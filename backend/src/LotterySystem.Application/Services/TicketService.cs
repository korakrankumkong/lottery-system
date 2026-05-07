using AutoMapper;
using LotterySystem.Application.Common;
using LotterySystem.Application.DTOs;
using LotterySystem.Application.Interfaces;
using LotterySystem.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace LotterySystem.Application.Services;

public sealed class TicketService(
    ITicketRepository ticketRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ILogger<TicketService> logger) : ITicketService
{
    public async Task<TicketDto> CreateAsync(CreateTicketRequestDto request, CancellationToken cancellationToken = default)
    {
        var ticket = new Ticket
        {
            CustomerId = request.CustomerId,
            LotteryRoundId = request.LotteryRoundId,
            Items = request.Items.Select(i => new TicketItem { Number = i.Number, Amount = i.Amount }).ToList()
        };
        ticket.TotalAmount = ticket.Items.Sum(x => x.Amount);

        await ticketRepository.AddAsync(ticket, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Ticket {TicketId} created with {ItemCount} items", ticket.Id, ticket.Items.Count);
        return mapper.Map<TicketDto>(ticket);
    }

    public async Task<TicketDto> UpdateAsync(Guid id, UpdateTicketRequestDto request, CancellationToken cancellationToken = default)
    {
        var ticket = await ticketRepository.GetByIdAsync(id, cancellationToken);
        if (ticket is null) throw new NotFoundException("Ticket not found");

        ticket.CustomerId = request.CustomerId;
        ticket.LotteryRoundId = request.LotteryRoundId;
        ticket.Items.Clear();
        ticket.Items = request.Items.Select(i => new TicketItem { Number = i.Number, Amount = i.Amount }).ToList();
        ticket.TotalAmount = ticket.Items.Sum(x => x.Amount);

        ticketRepository.Update(ticket);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Ticket {TicketId} updated", id);
        return mapper.Map<TicketDto>(ticket);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var ticket = await ticketRepository.GetByIdAsync(id, cancellationToken);
        if (ticket is null) throw new NotFoundException("Ticket not found");

        ticketRepository.Remove(ticket);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Ticket {TicketId} deleted", id);
    }

    public async Task<List<TicketDto>> GetHistoryAsync(TicketHistoryQueryDto query, CancellationToken cancellationToken = default)
    {
        var tickets = await ticketRepository.GetHistoryAsync(query.CustomerId, query.LotteryRoundId, query.FromUtc, query.ToUtc, query.Page, query.PageSize, cancellationToken);
        return mapper.Map<List<TicketDto>>(tickets);
    }

    public async Task<List<TicketDto>> SearchAsync(TicketSearchQueryDto query, CancellationToken cancellationToken = default)
    {
        var tickets = await ticketRepository.SearchAsync(query.Keyword, query.FromUtc, query.ToUtc, cancellationToken);
        return mapper.Map<List<TicketDto>>(tickets);
    }

    public async Task<List<DailySummaryDto>> GetDailySummaryAsync(DateTime fromUtc, DateTime toUtc, CancellationToken cancellationToken = default)
    {
        if (toUtc < fromUtc) throw new BadRequestException("Invalid date range");
        var rows = await ticketRepository.GetDailySummaryAsync(fromUtc, toUtc, cancellationToken);
        return rows.Select(x => new DailySummaryDto(x.Date, x.TicketCount, x.TotalAmount)).ToList();
    }

    public async Task<List<RiskyNumberDto>> GetRiskyNumbersAsync(DateTime fromUtc, DateTime toUtc, int top = 20, CancellationToken cancellationToken = default)
    {
        if (top <= 0 || top > 100) throw new BadRequestException("Top must be between 1 and 100");
        var rows = await ticketRepository.GetRiskyNumbersAsync(fromUtc, toUtc, top, cancellationToken);
        return rows.Select(x => new RiskyNumberDto(x.Number, x.Frequency, x.TotalAmount)).ToList();
    }

    public async Task<TotalsDto> GetTotalsAsync(DateTime? fromUtc, DateTime? toUtc, CancellationToken cancellationToken = default)
    {
        var totals = await ticketRepository.GetTotalsAsync(fromUtc, toUtc, cancellationToken);
        return new TotalsDto(totals.TicketCount, totals.ItemCount, totals.TotalAmount);
    }
}

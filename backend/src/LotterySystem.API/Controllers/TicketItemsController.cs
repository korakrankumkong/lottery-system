using LotterySystem.Application.DTOs;
using LotterySystem.Application.Interfaces;
using LotterySystem.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LotterySystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class TicketItemsController(IRepository<TicketItem> repository) : ControllerBase
{
    [HttpGet]
    public async Task<List<TicketItemDto>> Get(CancellationToken ct)
    {
        var items = await repository.GetAllAsync(ct);
        return items.Select(i => new TicketItemDto(i.Id, i.Number, i.Amount)).ToList();
    }
}

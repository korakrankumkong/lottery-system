using LotterySystem.Application.Interfaces;
using LotterySystem.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LotterySystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "StaffOrAdmin")]
public sealed class LotteryRoundsController(IRepository<LotteryRound> repository) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var rounds = await repository.GetAllAsync(cancellationToken);
        var result = rounds
            .OrderByDescending(x => x.DrawDate)
            .Select(x => new
            {
                id = x.Id,
                code = x.Code,
                drawDate = x.DrawDate.ToString("yyyy-MM-dd"),
                isClosed = x.IsClosed
            })
            .ToList();

        return Ok(result);
    }
}


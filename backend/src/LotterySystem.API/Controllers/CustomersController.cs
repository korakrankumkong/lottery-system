using LotterySystem.Application.Interfaces;
using LotterySystem.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LotterySystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "StaffOrAdmin")]
public sealed class CustomersController(IRepository<Customer> repository) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var customers = await repository.GetAllAsync(cancellationToken);
        var result = customers
            .OrderBy(x => x.Name)
            .Select(x => new
            {
                id = x.Id,
                name = x.Name,
                phoneNumber = x.PhoneNumber
            })
            .ToList();

        return Ok(result);
    }
}


using LotterySystem.Application.Interfaces;
using LotterySystem.Domain.Entities;
using LotterySystem.Domain.Enums;
using LotterySystem.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;

namespace LotterySystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class SeedController(IRepository<User> users, IUnitOfWork unitOfWork, LotteryDbContext dbContext) : ControllerBase
{
    [HttpPost("admin")]
    public async Task<IActionResult> SeedAdmin(CancellationToken cancellationToken)
    {
        await dbContext.Database.EnsureCreatedAsync(cancellationToken);

        var list = await users.GetAllAsync(cancellationToken);
        if (list.Any(x => x.Username == "admin"))
        {
            return Ok("Admin already exists");
        }

        await users.AddAsync(new User
        {
            Username = "admin",
            FullName = "System Admin",
            Role = UserRole.Admin,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123")
        }, cancellationToken);

        await users.AddAsync(new User
        {
            Username = "staff",
            FullName = "System Staff",
            Role = UserRole.Staff,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Staff@123")
        }, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Ok("Seeded users: admin/Admin@123 and staff/Staff@123");
    }
}

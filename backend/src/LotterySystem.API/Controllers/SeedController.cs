using LotterySystem.Application.Interfaces;
using LotterySystem.Domain.Entities;
using LotterySystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace LotterySystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class SeedController(IRepository<User> users, IUnitOfWork unitOfWork, LotteryDbContext dbContext) : ControllerBase
{
    [HttpPost("admin")]
    public async Task<IActionResult> SeedAdmin(CancellationToken cancellationToken)
    {
        _ = users;
        _ = unitOfWork;

        await dbContext.Database.ExecuteSqlRawAsync(
            """
            CREATE TABLE IF NOT EXISTS "Users" (
              "Id" uuid NOT NULL PRIMARY KEY,
              "Username" text NOT NULL,
              "PasswordHash" text NOT NULL,
              "FullName" text NOT NULL,
              "Role" integer NOT NULL,
              "RefreshTokenHash" text NULL,
              "RefreshTokenExpiresAtUtc" timestamp with time zone NULL,
              "CreatedAtUtc" timestamp with time zone NOT NULL,
              "UpdatedAtUtc" timestamp with time zone NULL,
              "DeletedAtUtc" timestamp with time zone NULL,
              "IsDeleted" boolean NOT NULL DEFAULT FALSE
            );
            CREATE UNIQUE INDEX IF NOT EXISTS "IX_Users_Username" ON "Users" ("Username");
            """);

        var adminHash = BCrypt.Net.BCrypt.HashPassword("Admin@123");
        var staffHash = BCrypt.Net.BCrypt.HashPassword("Staff@123");

        await dbContext.Database.ExecuteSqlRawAsync(
            """
            INSERT INTO "Users" ("Id","Username","PasswordHash","FullName","Role","CreatedAtUtc","IsDeleted")
            VALUES
              ({0}, 'admin', {1}, 'System Admin', 1, {2}, FALSE),
              ({3}, 'staff', {4}, 'System Staff', 2, {5}, FALSE)
            ON CONFLICT ("Username") DO NOTHING;
            """,
            Guid.NewGuid(), adminHash, DateTime.UtcNow,
            Guid.NewGuid(), staffHash, DateTime.UtcNow);

        return Ok("Seed complete. admin/Admin@123 and staff/Staff@123 are ready.");
    }
}

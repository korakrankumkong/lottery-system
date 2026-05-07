using System.Text;
using LotterySystem.Application.Interfaces;
using LotterySystem.Infrastructure.Configuration;
using LotterySystem.Infrastructure.Persistence;
using LotterySystem.Infrastructure.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace LotterySystem.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var dbOptions = configuration.GetSection(DatabaseOptions.SectionName).Get<DatabaseOptions>() ?? new DatabaseOptions();

        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is missing.");

        services.AddDbContextPool<LotteryDbContext>((serviceProvider, options) =>
        {
            options.UseNpgsql(connectionString, npgsql =>
            {
                npgsql.MigrationsAssembly(typeof(LotteryDbContext).Assembly.FullName);
                npgsql.CommandTimeout(dbOptions.CommandTimeoutSeconds);
                npgsql.EnableRetryOnFailure(dbOptions.MaxRetryCount, TimeSpan.FromSeconds(dbOptions.MaxRetryDelaySeconds), null);
            });

            if (dbOptions.EnableDetailedErrors) options.EnableDetailedErrors();
            if (dbOptions.EnableSensitiveDataLogging) options.EnableSensitiveDataLogging();
        }, dbOptions.PoolSize);

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<ITicketRepository, TicketRepository>();
        services.AddScoped<IAuthRepository, AuthRepository>();
        services.AddScoped<IReportRepository, ReportRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<ITokenService, JwtTokenService>();

        var jwt = configuration.GetSection("Jwt");
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    ValidIssuer = jwt["Issuer"],
                    ValidAudience = jwt["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!)),
                    ClockSkew = TimeSpan.FromMinutes(1)
                };
            });

        services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminOnly", p => p.RequireRole("Admin"));
            options.AddPolicy("StaffOrAdmin", p => p.RequireRole("Admin", "Staff"));
        });

        return services;
    }
}

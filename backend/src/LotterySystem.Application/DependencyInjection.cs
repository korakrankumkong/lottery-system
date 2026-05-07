using FluentValidation;
using LotterySystem.Application.Common;
using LotterySystem.Application.Interfaces;
using LotterySystem.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace LotterySystem.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ITicketService, TicketService>();
        services.AddScoped<IReportService, ReportService>();
        services.AddAutoMapper(typeof(MappingProfile).Assembly);
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
        return services;
    }
}

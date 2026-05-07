using LotterySystem.Infrastructure.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace LotterySystem.Infrastructure.Persistence;

public sealed class LotteryDbContextFactory : IDesignTimeDbContextFactory<LotteryDbContext>
{
    public LotteryDbContext CreateDbContext(string[] args)
    {
        var basePath = Directory.GetCurrentDirectory();

        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var dbOptions = configuration.GetSection(DatabaseOptions.SectionName).Get<DatabaseOptions>() ?? new DatabaseOptions();
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");

        var optionsBuilder = new DbContextOptionsBuilder<LotteryDbContext>();
        optionsBuilder.UseNpgsql(connectionString, npgsql =>
        {
            npgsql.MigrationsAssembly(typeof(LotteryDbContext).Assembly.FullName);
            npgsql.CommandTimeout(dbOptions.CommandTimeoutSeconds);
            npgsql.EnableRetryOnFailure(
                dbOptions.MaxRetryCount,
                TimeSpan.FromSeconds(dbOptions.MaxRetryDelaySeconds),
                errorCodesToAdd: null);
        });

        return new LotteryDbContext(optionsBuilder.Options);
    }
}

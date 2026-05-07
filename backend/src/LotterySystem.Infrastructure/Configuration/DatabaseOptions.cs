namespace LotterySystem.Infrastructure.Configuration;

public sealed class DatabaseOptions
{
    public const string SectionName = "Database";

    public int CommandTimeoutSeconds { get; set; } = 30;
    public bool EnableDetailedErrors { get; set; }
    public bool EnableSensitiveDataLogging { get; set; }
    public int MaxRetryCount { get; set; } = 5;
    public int MaxRetryDelaySeconds { get; set; } = 15;
    public int PoolSize { get; set; } = 128;
}

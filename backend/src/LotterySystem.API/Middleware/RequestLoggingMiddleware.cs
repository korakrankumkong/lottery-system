namespace LotterySystem.API.Middleware;

public sealed class RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
{
    public async Task Invoke(HttpContext context)
    {
        var started = DateTime.UtcNow;
        await next(context);
        var elapsed = (DateTime.UtcNow - started).TotalMilliseconds;

        logger.LogInformation("{Method} {Path} responded {StatusCode} in {ElapsedMs}ms",
            context.Request.Method,
            context.Request.Path,
            context.Response.StatusCode,
            Math.Round(elapsed, 2));
    }
}

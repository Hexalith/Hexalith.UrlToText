namespace WebCrawler;

using Serilog;

public static class ErrorHandler
{
    private static readonly ILogger _logger = new LoggerConfiguration()
        .WriteTo.File("log.txt", rollingInterval: RollingInterval.Day)
        .CreateLogger();

    public static void LogError(Exception ex, string context) => _logger.Error(ex, "Error occurred in {Context}", context);

    public static void LogWarning(string message, string context) => _logger.Warning("{Message} in {Context}", message, context);

    public static void LogInfo(string message) => _logger.Information(message);
}

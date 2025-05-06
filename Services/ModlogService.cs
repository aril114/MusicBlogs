namespace MusicBlogs.Services;

using Serilog;
public class ModlogService
{
    private readonly ILogger _logger;

    public ModlogService()
    {
        _logger = new LoggerConfiguration()
            .WriteTo.File(
                path: "logs/mod.log")
            .CreateLogger();
    }

    public void LogAction(string action, string moderatorName, string details)
    {
        _logger.Information("""
            {action} | Модератор: {moderatorName} | {details}
            """,
            action, moderatorName, details);
    }
}
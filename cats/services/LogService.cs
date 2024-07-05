using cats.Data;
using cats.Models;

namespace cats.Services;

public class LogService : ILogService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<LogService> _logger;

    public LogService(ApplicationDbContext context, ILogger<LogService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task LogAsync(string userName, string requestUrl, string description)
    {
        try
        {
            var logEntry = new LogEntry
            {
                UserName = userName,
                RequestUrl = requestUrl,
                Description = description,
                Timestamp = DateTime.UtcNow
            };

            _context.LogEntries.Add(logEntry);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to log operation");
            throw; 
        }
    }
}
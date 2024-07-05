namespace cats.Services;

public interface ILogService
{
    Task LogAsync(string userName, string requestUrl, string description);
}
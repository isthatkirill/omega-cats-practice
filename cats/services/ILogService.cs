namespace cats.services;

public interface ILogService
{
    Task LogAsync(string userName, string requestUrl, string description);
}
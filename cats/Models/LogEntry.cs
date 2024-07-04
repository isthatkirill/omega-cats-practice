namespace cats.Models;

public class LogEntry
{
    public int Id { get; set; }
    public string UserName { get; set; } 
    public string RequestUrl { get; set; } 
    public string Description { get; set; } 
    public DateTime Timestamp { get; set; } 
}
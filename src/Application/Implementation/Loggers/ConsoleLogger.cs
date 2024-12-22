using Application.Abstraction.Interfaces;

namespace Application.Implementation.Loggers;

public class ConsoleLogger : ILogger
{
    public void Log(string message)
    {
        Console.WriteLine($"[Console Logger] {message}");
    }

    public void LogError(Exception exception, string message)
    {
        Console.WriteLine($"[Console Logger] ERROR: {message}");
        Console.WriteLine($"Exception: {exception.GetType().Name} - {exception.Message}");
        Console.WriteLine($"StackTrace: {exception.StackTrace}");
    }
}
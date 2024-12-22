using Application.Abstraction.Interfaces;

namespace Application.Implementation.Loggers;

public class ConsoleLogger : ILogger
{
    public void Log(string message)
    {
        Console.WriteLine($"[Console Logger] {message}");
    }
}
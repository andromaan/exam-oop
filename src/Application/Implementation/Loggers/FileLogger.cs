using Application.Abstraction.Interfaces;

namespace Application.Implementation.Loggers;

public class FileLogger : ILogger
{
    private readonly string _filePath;

    public FileLogger(string filePath)
    {
        _filePath = filePath;
        
        var directory = Path.GetDirectoryName(_filePath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
    }

    public void Log(string message)
    {
        using (var writer = new StreamWriter(_filePath, append: true))
        {
            writer.WriteLine($"[File Logger] {DateTime.UtcNow}: {message}");
        }
    }

    public void LogError(Exception exception, string message)
    {
        using (var writer = new StreamWriter(_filePath, append: true))
        {
            writer.WriteLine($"[File Logger] {DateTime.UtcNow}: ERROR - {message}");
            writer.WriteLine($"Exception: {exception.GetType().Name} - {exception.Message}");
            writer.WriteLine($"StackTrace: {exception.StackTrace}");
        }
    }
}
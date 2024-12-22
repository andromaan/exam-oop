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
            writer.WriteLine($"[File Logger] {DateTime.Now}: {message}");
        }
    }
}
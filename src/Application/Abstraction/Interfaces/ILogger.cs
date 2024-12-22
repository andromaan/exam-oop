namespace Application.Abstraction.Interfaces;

public interface ILogger
{
    void Log(string message);
    void LogError(Exception exception, string anUnexpectedErrorOccurred);
}
using Application.Abstraction.Interfaces;
using Application.Implementation.Loggers;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Factiories;

public static class LoggerFactory
{
    public static ILogger CreateLogger(IConfiguration configuration)
    {
        var loggerType = configuration["Logging:LoggerType"];

        return loggerType switch
        {
            "Console" => new ConsoleLogger(),
            "File" => new FileLogger(configuration["Logging:FilePath"] ?? "log.txt"),
            _ => throw new InvalidOperationException("Invalid logger type specified in configuration.")
        };
    }
}

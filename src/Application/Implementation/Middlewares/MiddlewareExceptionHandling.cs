using System.Net;
using System.Text.Json;
using Application.Abstraction.Interfaces;
using Application.Implementation.PayrollManager;
using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace Application.Implementation.Middlewares;

public class MiddlewareExceptionHandling(RequestDelegate next, ILogger logger)
{
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        logger.LogError(exception, "An unexpected error occurred");

        var response = context.Response;
        response.ContentType = "application/json";

        var statusCode = exception switch
        {
            FileNotFoundException 
                or EmployeeNotFoundException
                or TransactionNotFoundException => (int)HttpStatusCode.NotFound,
            UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized,
            InvalidOperationException 
                or DatePeriodInvalidException
                or ValidationException
                or ArgumentException => (int)HttpStatusCode.BadRequest,
            _ => (int)HttpStatusCode.InternalServerError
        };

        response.StatusCode = statusCode;

        var errorResponse = new
        {
            StatusCode = statusCode,
            Message = exception.Message,
            Details = exception.InnerException?.Message
        };

        await response.WriteAsync(JsonSerializer.Serialize(errorResponse));
    }
}
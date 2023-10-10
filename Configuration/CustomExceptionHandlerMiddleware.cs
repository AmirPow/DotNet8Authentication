using System.Net;
using System.Text.Json;

namespace DotNet8Authentication.Configuration;

public class CustomExceptionHandlerMiddleware : IMiddleware
{
    private readonly ILogger<CustomExceptionHandlerMiddleware> logger;

    public CustomExceptionHandlerMiddleware(
        ILogger<CustomExceptionHandlerMiddleware> logger)
    {
        this.logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        string exceptionMessage;
        string result;

        try
        {
            await next(context);
        }
        //catch (DomainException exception)
        //{
        //    exceptionMessage = exception.Message;

        //    logger.LogError(exception, $"ِDomain Exception: {exceptionMessage}");

        //    result = JsonSerializer.Serialize(exceptionMessage);

        //    await WriteToResponseAsync();
        //}
        catch (Exception exception)
        {
            exceptionMessage = "خطایی رخ داد";

            if (exception.Message.Contains("Sequence contains no elements"))
                exceptionMessage = "رکوردی یافت نشد";

            logger.LogError(exception, $"Application Exception: {exceptionMessage}");

            result = JsonSerializer.Serialize(exceptionMessage);

            await WriteToResponseAsync();
        }

        async Task WriteToResponseAsync()
        {
            if (context.Response.HasStarted)
                throw new InvalidOperationException("The response has already started, the http status code middleware will not be executed.");

            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(result);
        }
    }
}
using ChatRoom.Rest.Api.DataTransferObject;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ChatRoom.Rest.Api.Handlers;

public class ExceptionHandlerMiddleware
{
    private readonly ILogger<ExceptionHandlerMiddleware> _logger;
    private readonly RequestDelegate _next;

    public ExceptionHandlerMiddleware(ILogger<ExceptionHandlerMiddleware> logger, RequestDelegate next)
    {
        _logger = logger;
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next.Invoke(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionResponse(ex, context);
        }
    }

    private async Task HandleExceptionResponse(Exception ex, HttpContext context)
    {
        var response = context.Response;
        response.ContentType = "application/json";

        response.StatusCode = ex switch
        {
            NullReferenceException => StatusCodes.Status400BadRequest,
            InvalidOperationException => StatusCodes.Status400BadRequest,
            CosmosException => StatusCodes.Status400BadRequest,
            _ => StatusCodes.Status500InternalServerError
        };

        var message = ex switch
        {
            NullReferenceException => "Bad request",
            InvalidOperationException => "Bad request",
            CosmosException => "Bad request",
            _ => "Internal server error"
        };

        _logger.LogError(ex, ex.Message + " - type {ARGO}", ex.GetType());

        var result = JsonConvert.SerializeObject(new ErrorResult(message));
        await context.Response.WriteAsync(result);
    }
}
using Microsoft.AspNetCore.Builder;

namespace ChatRoom.Rest.Api.Handlers;

public static class HandlersExtensions
{
    public static IApplicationBuilder UseExceptionHandlerMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ExceptionHandlerMiddleware>();
    }
}
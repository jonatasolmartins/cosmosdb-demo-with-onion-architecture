using ChatRoom.Core.Application.Services.Implementations;
using ChatRoom.Core.Domain.Abstractions.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ChatRoom.Core.Application.Services.DependencyInjection;

public static  class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped(typeof(IMessageService), typeof(MessageService));
        return services;
    }
    
}
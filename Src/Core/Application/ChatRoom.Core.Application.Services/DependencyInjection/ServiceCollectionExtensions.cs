using ChatRoom.Core.Application.Services.Implementations;
using ChatRoom.Core.Domain.Abstractions.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ChatRoom.Core.Application.Services.DependencyInjection;

public static  class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddSingleton(typeof(IMessageService), typeof(MessageService));
        services.AddSingleton(typeof(IUserService), typeof(UserService));

        /*services.AddSingleton(s =>
        {
            var configuration = s.GetService<IConfiguration>();

            return new QueueClient(configuration.GetSection("ConnectionStrings:QueueConnection").Value,
                configuration.GetSection("ConnectionStrings:QueueName").Value);
        });
        
        services.AddHostedService<MessageProcessor>();
        */
        return services;
    }
    
}
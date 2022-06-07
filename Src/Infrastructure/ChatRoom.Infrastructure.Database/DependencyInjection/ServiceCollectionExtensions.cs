using ChatRoom.Core.Domain.Abstractions.Repositories;
using ChatRoom.Infrastructure.Database.Data;
using ChatRoom.Infrastructure.Database.Implementations;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ChatRoom.Infrastructure.Database.DependencyInjection;

public static  class ServiceCollectionExtensions
{
    public static IServiceCollection AddDatabaseRepositories(this IServiceCollection services)
    {

        services.AddSingleton(typeof(IRepository), typeof(Repository));
        services.AddCosmosDbClient();
        return services;
    }

    private static IServiceCollection AddCosmosDbClient(this IServiceCollection services)
    {
        services.AddSingleton<CosmosClient>(s =>
        {
            var configurationSection = s.GetRequiredService<IConfiguration>().GetSection("CosmosConnection");

            return CosmosDbClientSetup.Setup(configurationSection).GetAwaiter().GetResult();
        });
        
        return services;
    }
    
}
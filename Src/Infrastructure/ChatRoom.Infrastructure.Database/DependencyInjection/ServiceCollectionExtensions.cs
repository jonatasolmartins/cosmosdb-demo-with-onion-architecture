using Azure.Storage.Blobs;
using ChatRoom.Core.Domain.Abstractions.Repositories;
using ChatRoom.Infrastructure.Database.AppSettings;
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
        services.AddSingleton(s =>
        {
            var configuration = s.GetService<IConfiguration>();
            return new BlobServiceClient(configuration.GetSection("AzureBlobStorageConnection").Value);
        });

        services.AddSingleton(typeof(IMultimediaRepository), typeof(MultimediaRepository));
        services.AddCosmosDbClient();
        return services;
    }

    private static IServiceCollection AddCosmosDbClient(this IServiceCollection services)
    {

        services.AddSingleton<CosmosClient>(s =>
        {
            var cosmoDbSettings = s.GetService<CosmoDbSettings>();
            return CosmosDbClientSetup.Setup(cosmoDbSettings).GetAwaiter().GetResult();
        });
           
        
        return services;
    }
    
}
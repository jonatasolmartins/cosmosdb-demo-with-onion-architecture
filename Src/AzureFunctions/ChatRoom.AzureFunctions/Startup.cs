using System.IO;
using ChatRoom.Core.Domain.Abstractions.Repositories;
using ChatRoom.Infrastructure.Database.AppSettings;
using ChatRoom.Infrastructure.Database.Implementations;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly:FunctionsStartup(typeof(ChatRoom.AzureFunctions.Startup))]
namespace ChatRoom.AzureFunctions;
public class Startup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile($"local.settings.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();
        
        builder.Services.AddSingleton<IConfiguration>(configuration);

        var cosmosDbConfig = configuration.GetSection("ConnectionStrings:CosmoDbCredentials").Get<CosmoDbSettings>();
        builder.Services.AddSingleton(cosmosDbConfig);
        
        CosmosClientBuilder clientBuilder = new CosmosClientBuilder(cosmosDbConfig.AccountEndpointUrl, cosmosDbConfig.PrimaryKey);
        
        CosmosClient cosmoDbClient = clientBuilder
            .WithApplicationName(cosmosDbConfig.DatabaseName)
            .WithApplicationName(Regions.EastUS)
            .WithConnectionModeDirect()
            .WithSerializerOptions(new CosmosSerializationOptions()
                {PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase})
            .Build();
        
        builder.Services.AddSingleton(cosmoDbClient);
        
        builder.Services.AddSingleton<IRepository, Repository>();
    }
}
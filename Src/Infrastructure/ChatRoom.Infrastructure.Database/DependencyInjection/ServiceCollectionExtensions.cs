using ChatRoom.Core.Domain.Abstractions.Repositories;
using ChatRoom.Core.Domain.Models;
using ChatRoom.Infrastructure.Database.Implementations;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ChatRoom.Infrastructure.Database.DependencyInjection;

public static  class ServiceCollectionExtensions
{
    public static IServiceCollection AddDatabaseRepositories(this IServiceCollection services)
    {
        services.AddSingleton<IRoomRepository, RoomRepository>();
        services.AddCosmosDbClient();
        return services;
    }

    private static IServiceCollection AddCosmosDbClient(this IServiceCollection services)
    {
        services.AddSingleton<CosmosClient>(s =>
        {
            var configurationSection = s.GetRequiredService<IConfiguration>().GetSection("CosmosConnection");
            var databaseName = configurationSection.GetSection("DatabaseName").Value;
            var accountEndPoint = configurationSection.GetSection("Account").Value;
            var key = configurationSection.GetSection("Key").Value;
            
            CosmosClientBuilder clientBuilder = new CosmosClientBuilder(accountEndPoint, key);
            CosmosClient client = clientBuilder
                .WithApplicationName(databaseName)
                .WithApplicationName(Regions.EastUS)
                .WithConnectionModeDirect()
                .WithSerializerOptions(new CosmosSerializationOptions()
                    {PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase})
                .Build();

            Microsoft.Azure.Cosmos.Database database =  client.CreateDatabaseIfNotExistsAsync(databaseName).Result;

            var containerProperties = new ContainerProperties()
            {
                Id = "Room",
                PartitionKeyPath = "/id"
            };

            Container roomContainer =  database.CreateContainerIfNotExistsAsync(containerProperties).Result;
        
            foreach (var room in SeedData())
            {
                _ = roomContainer.CreateItemAsync(room, new PartitionKey(room.Id.ToString()));
            }

            return client;
        });
        
        return services;
    }
    
    private static List<Room> SeedData()
    {
        return new List<Room>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Jala",
                Chats = new List<Chat>
                {
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Name = "BootCampers",
                        Messages = new List<Message>
                        {
                            new()
                            {
                                Id = Guid.NewGuid(),
                                Description = "Well-come!"
                            }
                        }
                    }
                }
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "JalaFoundation",
                Chats = new List<Chat>
                {
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Name = "General",
                        Messages = new List<Message>
                        {
                            new()
                            {
                                Id = Guid.NewGuid(),
                                Description = "Hey there!"
                            }
                        }
                    }
                }
            }
        };
    }
}
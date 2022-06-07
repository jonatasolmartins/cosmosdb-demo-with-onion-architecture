using ChatRoom.Core.Domain.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Azure.Cosmos.Scripts;
using Microsoft.Extensions.Configuration;

namespace ChatRoom.Infrastructure.Database.Data;

public static class CosmosDbClientSetup
{
    private const string PathToConvertDateFunction =
        @"../../../..//Infrastructure/ChatRoom.Infrastructure.Database/Data/CosmosDbScripts/UserDefinedFunctions/convertDate.js";
    public static async Task<CosmosClient> Setup(IConfigurationSection configurationSection)
    {
        var databaseName = configurationSection.GetSection("DatabaseName").Value;
        var key = configurationSection.GetSection("Key").Value;
        var accountEndPoint = configurationSection.GetSection("Account").Value;
        
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
            
        await CreateUserDefinedFunction(roomContainer, PathToConvertDateFunction);
            
        foreach (var room in SeedData())
        {
            _ = roomContainer.CreateItemAsync(room, new PartitionKey(room.Id.ToString()));
        }

        return client;
    }
    
    private static async Task CreateUserDefinedFunction(Container container, string scriptFilePath)
    {
        scriptFilePath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, scriptFilePath));

        var scriptId = Path.GetFileNameWithoutExtension(scriptFilePath);
        var properties = new UserDefinedFunctionProperties
        {
            Id = scriptId,
            Body = await File.ReadAllTextAsync(scriptFilePath)
        };
        
        if (await UserDefinedFunctionExists(container, scriptId))
        {
            
            await container.Scripts.ReplaceUserDefinedFunctionAsync(properties);
            return;
        }
        
        await container.Scripts.CreateUserDefinedFunctionAsync(properties);
            
    }
    
    private static async Task<bool> UserDefinedFunctionExists(Container container, string udfId)
    {
        var cosmosScripts = container.Scripts;

        try
        {
            var userDefinedFunction = await cosmosScripts.ReadUserDefinedFunctionAsync(udfId);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return false;
        }
    }
    
    private static async Task UpsertStoredProcedureAsync(Container container, string scriptFileName)
    {
        string scriptId = Path.GetFileNameWithoutExtension(scriptFileName);
        if (await StoredProcedureExists(container, scriptId))
        {
            await container.Scripts.ReplaceStoredProcedureAsync(new StoredProcedureProperties(scriptId, File.ReadAllText(scriptFileName)));
        }
        else
        {
            await container.Scripts.CreateStoredProcedureAsync(new StoredProcedureProperties(scriptId, File.ReadAllText(scriptFileName)));
        }
    }


    private static async Task<bool> StoredProcedureExists(Container container, string sprocId)
    {
        Scripts cosmosScripts = container.Scripts;

        try
        {
            StoredProcedureResponse sproc = await cosmosScripts.ReadStoredProcedureAsync(sprocId);
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }


        private static async Task UpsertTriggerAsync(Container container, string scriptFileName, TriggerOperation triggerOperation, TriggerType triggerType)
        {
            string scriptId = Path.GetFileNameWithoutExtension(scriptFileName);
            if (await TriggerExists(container, scriptId))
            {
                await container.Scripts.ReplaceTriggerAsync(new TriggerProperties { Id = scriptId, Body = File.ReadAllText(scriptFileName), TriggerOperation = triggerOperation, TriggerType = triggerType });
            }
            else
            {
                await container.Scripts.CreateTriggerAsync(new TriggerProperties { Id = scriptId, Body = File.ReadAllText(scriptFileName), TriggerOperation = triggerOperation, TriggerType = triggerType });
            }

        }


        private static async Task<bool> TriggerExists(Container container, string sprocId)
        {
            Scripts cosmosScripts = container.Scripts;

            try
            {
                TriggerResponse trigger = await cosmosScripts.ReadTriggerAsync(sprocId);
                return true;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return false;
            }
        }
    private static List<Room> SeedData()
    {
        return new List<Room>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Name = "BootCampers",
                DateCreated = DateTime.UtcNow.ToString(),
                Chats = new List<Chat>
                {
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Name = "BootCampers",
                        DateCreated = DateTime.UtcNow,
                        Messages = new List<Message>
                        {
                            new()
                            {
                                Id = Guid.NewGuid(),
                                DateCreated = DateTime.UtcNow,
                                Description = "Well-come!"
                            }
                        }
                    }
                }
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "General",
                DateCreated = DateTime.UtcNow.ToString(),
                Chats = new List<Chat>
                {
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Name = "General",
                        DateCreated = DateTime.UtcNow,
                        Messages = new List<Message>
                        {
                            new()
                            {
                                Id = Guid.NewGuid(),
                                DateCreated = DateTime.UtcNow,
                                Description = "Hey there!"
                            }
                        }
                    }
                }
            }
        };
    }

}
using ChatRoom.Core.Domain.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Azure.Cosmos.Scripts;
using Microsoft.Extensions.Configuration;

namespace ChatRoom.Infrastructure.Database.Data;

public static class CosmosDbClientSetup
{
    private const string PathToFileJs =
        @"../../../..//Infrastructure/ChatRoom.Infrastructure.Database/Data/";
    
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

        var roomContainerProperties = new ContainerProperties()
        {
            Id = "Room",
            PartitionKeyPath = "/id"
        };

        Container roomContainer =  database.CreateContainerIfNotExistsAsync(roomContainerProperties).Result;
        
        var messageContainerProperties = new ContainerProperties()
        {
            Id = "Message",
            PartitionKeyPath = "/id"
        };

        foreach (var room in SeedData())
        {
            _ = await roomContainer.CreateItemAsync(room);
        }
        
        _ =  database.CreateContainerIfNotExistsAsync(messageContainerProperties).Result;
        await CreateUserDefinedFunction(roomContainer, $"{PathToFileJs}{configurationSection.GetSection("udf_convertDate").Value}");
        await UpsertStoredProcedureAsync(roomContainer, $"{PathToFileJs}{configurationSection.GetSection("sp_updateMessage").Value}");
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
    
    private static async Task UpsertStoredProcedureAsync(Container container, string scriptFilePath)
    {
        scriptFilePath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, scriptFilePath));

        var scriptId = Path.GetFileNameWithoutExtension(scriptFilePath);
        if (await StoredProcedureExists(container, scriptId))
        {
            await container.Scripts.ReplaceStoredProcedureAsync(new StoredProcedureProperties(scriptId, File.ReadAllText(scriptFilePath)));
        }
        else
        {
            await container.Scripts.CreateStoredProcedureAsync(new StoredProcedureProperties(scriptId, File.ReadAllText(scriptFilePath)));
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
        var room = new Room
        {
            Id = Guid.NewGuid(),
            Name = "BootCampers",
            DateCreated = DateTime.UtcNow.ToString(),
            Chats = new List<Chat>()
        };
   
        var chat = new Chat
        {
            Id = Guid.NewGuid(),
            RoomId = room.Id,
            Name = "BootCampers",
            DateCreated = DateTime.UtcNow,
            Messages = new List<Message>()
        };

        var chat2 = new Chat
        {
            Id = Guid.NewGuid(),
            RoomId = room.Id,
            Name = "Trainers",
            DateCreated = DateTime.UtcNow,
            Messages = new List<Message>()
        };
        
        var message = new Message
        {
            Id = Guid.NewGuid(),
            DateCreated = DateTime.UtcNow,
            ChatId = chat.Id,
            Description = "Well-come!"
        }; 

        var message2 = new Message
        {
            Id = Guid.NewGuid(),
            DateCreated = DateTime.UtcNow,
            ChatId = chat2.Id,
            Description = "Hey there!"
        }; 
        
        chat.Messages.Add(message);
        chat2.Messages.Add(message2);
        
        room.Chats.Add(chat);   
        room.Chats.Add(chat2);

        var data = new List<Room> {room};
        return data;
    }

}
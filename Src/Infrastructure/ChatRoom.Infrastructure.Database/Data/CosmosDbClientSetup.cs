using System.Net;
using ChatRoom.Core.Domain.Models;
using ChatRoom.Infrastructure.Database.AppSettings;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Azure.Cosmos.Scripts;
using User = ChatRoom.Core.Domain.Models.User;

namespace ChatRoom.Infrastructure.Database.Data;

public class CosmosDbClientSetup
{
    private const string PathToFileJs =
        @"../../../..//Infrastructure/ChatRoom.Infrastructure.Database/Data/";
    
    public static async Task<CosmosClient> Setup(CosmoDbSettings cosmoDbSettings)
    {

        CosmosClientBuilder clientBuilder = new CosmosClientBuilder(cosmoDbSettings.AccountEndpointUrl, cosmoDbSettings.PrimaryKey);
        
        CosmosClient client = clientBuilder
            .WithApplicationName(cosmoDbSettings.DatabaseName)
            .WithApplicationName(Regions.EastUS)
            .WithConnectionModeDirect()
            .WithSerializerOptions(new CosmosSerializationOptions()
                {PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase})
            .Build();

        Microsoft.Azure.Cosmos.Database database =  client.CreateDatabaseIfNotExistsAsync(cosmoDbSettings.DatabaseName).Result;

        var roomContainerProperties = new ContainerProperties()
        {
            Id = "Room",
            PartitionKeyPath = "/id"
        };

        Container roomContainer = await  database.CreateContainerIfNotExistsAsync(roomContainerProperties);
        
        var userContainerProperties = new ContainerProperties()
        {
            Id = "User",
            PartitionKeyPath = "/email"
        };

        Container _userContainer = await database.CreateContainerIfNotExistsAsync(userContainerProperties);
        
        var messageContainerProperties = new ContainerProperties()
        {
            Id = "Message",
            PartitionKeyPath = "/partitionKey"
        };

        Container _messageContainer = await database.CreateContainerIfNotExistsAsync(messageContainerProperties);


        var (room, messages, user) = SeedData();

        _ = await roomContainer.CreateItemAsync(room);
        _ = await _messageContainer.CreateItemAsync(messages.First());
        _ = await _messageContainer.CreateItemAsync(messages.Last());
        _ = await _userContainer.CreateItemAsync(user);
        
        await CreateUserDefinedFunction(roomContainer, $"{PathToFileJs}{cosmoDbSettings.UdfConvertDate}");
        await UpsertStoredProcedureAsync(roomContainer, $"{PathToFileJs}{cosmoDbSettings.ProcUpdateMessage}");
        await UpsertStoredProcedureAsync(roomContainer, $"{PathToFileJs}{cosmoDbSettings.ProcUpdateUserAvatar}");
        
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
            //await container.Scripts.ReplaceStoredProcedureAsync(new StoredProcedureProperties(scriptId, File.ReadAllText(scriptFilePath)));
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
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return false;
        }
    }

    private static ValueTuple<Room, List<Message>, User> SeedData()
    {

        var room = new Room
        {
            Id = Guid.NewGuid(),
            Name = "Latam",
            DateCreated = DateTime.UtcNow.ToString(),
            Chats = new List<Chat>()
        };

        var chat = new Chat
        {
            Id = Guid.NewGuid(),
            RoomId = room.Id,
            Name = "Latam",
            DateCreated = DateTime.UtcNow,
            Messages = new List<Message>()
        };

        var chat2 = new Chat
        {
            Id = Guid.NewGuid(),
            RoomId = room.Id,
            Name = "Brazil",
            DateCreated = DateTime.UtcNow,
            Messages = new List<Message>()
        };

        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = "Tiago",
            Email = "tiago@email.com",
            Avatar = "/image/2"
        };

        var message = new Message
        {
            Id = Guid.NewGuid(),
            DateCreated = DateTime.UtcNow,
            ChatId = chat.Id,
            RoomId = room.Id,
            Content = "Well-come!",
            User = user,
            PartitionKey = user.Email
        };



        var message2 = new Message
        {
            Id = Guid.NewGuid(),
            DateCreated = DateTime.UtcNow,
            ChatId = chat2.Id,
            RoomId = room.Id,
            Content = "Hey there!",
            User = user,
            PartitionKey = user.Email
        };

        chat.Messages.Add(message);
        chat2.Messages.Add(message2);

        room.Chats.Add(chat);
        room.Chats.Add(chat2);

        return new ValueTuple<Room, List<Message>, User>(room, new List<Message> {message, message2}, user);
    }

}
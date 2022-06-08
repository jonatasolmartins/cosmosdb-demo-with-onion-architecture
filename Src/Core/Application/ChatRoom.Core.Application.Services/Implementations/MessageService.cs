using System.Net;
using ChatRoom.Core.Domain.Abstractions.Services;
using ChatRoom.Core.Domain.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;

namespace ChatRoom.Core.Application.Services.Implementations;

public class MessageService : IMessageService
{
    private readonly Container _messageContainer;
    private readonly Container _roomContainer;
    public MessageService(CosmosClient cosmosClient, IConfiguration configuration)
    {
        var configurationSection = configuration.GetSection("CosmosConnection");
        string databaseName = configurationSection.GetSection("DatabaseName").Value;
        _messageContainer = cosmosClient.GetContainer(databaseName, "Message");
        _roomContainer = cosmosClient.GetContainer(databaseName, "Room");
    }
    public async Task<bool> CreateMessage(Message newMessage, Guid roomId)
    {
        return await UpsertMessage(newMessage, roomId);
    }

    public async Task<bool> UpdateMessage(Message message, Guid roomId)
    {
        return await UpsertMessage(message, roomId);
    }

    public async Task<bool> DeleteMessage(Guid id)
    {
         var response = await _messageContainer.DeleteItemAsync<Message>(id.ToString(), new PartitionKey(id.ToString()));
         return response.StatusCode == HttpStatusCode.OK;
    }

    private async Task<bool> UpsertMessage(Message message, Guid roomId)
    {
        var response = await _messageContainer.UpsertItemAsync<Message>(message, new PartitionKey(message.Id.ToString()));
        if (response.StatusCode != HttpStatusCode.Created) return false;
        
        var obj = new dynamic[] { message.ChatId.ToString(), message };
        var responseSp = await _roomContainer.Scripts.ExecuteStoredProcedureAsync<string>("updateMessage", new PartitionKey(roomId.ToString()), obj);
        return responseSp.StatusCode == HttpStatusCode.OK;

    }
}
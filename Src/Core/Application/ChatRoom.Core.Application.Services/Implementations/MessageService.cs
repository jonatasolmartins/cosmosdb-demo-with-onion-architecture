using System.Net;
using ChatRoom.Core.Domain.Abstractions.Services;
using ChatRoom.Core.Domain.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;

namespace ChatRoom.Core.Application.Services.Implementations;

public class MessageService : IMessageService
{
    private readonly Container _messageContainer;
    public MessageService(CosmosClient cosmosClient, IConfiguration configuration)
    {
        var databaseName = configuration.GetSection("ConnectionStrings:DatabaseName").Value;
        _messageContainer = cosmosClient.GetContainer(databaseName, "Message");
    }
    public async Task<bool> CreateMessage(Message newMessage)
    {
        return await UpsertMessage(newMessage);
    }

    public async Task<bool> UpdateMessage(Message message, Guid roomId)
    {
        return await UpsertMessage(message);
    }

    public async Task<bool> DeleteMessage(Guid id)
    {
         var response = await _messageContainer.DeleteItemAsync<Message>(id.ToString(), new PartitionKey(id.ToString()));
         return response.StatusCode == HttpStatusCode.OK;
    }

    private async Task<bool> UpsertMessage(Message message)
    {
        var response = await _messageContainer.UpsertItemAsync<Message>(message, new PartitionKey(message.Id.ToString()));
        return response.StatusCode == HttpStatusCode.Created;
    }
}
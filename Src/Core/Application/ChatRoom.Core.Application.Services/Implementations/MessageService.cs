using System.Net;
using ChatRoom.Core.Domain.Abstractions.Services;
using ChatRoom.Core.Domain.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using User = ChatRoom.Core.Domain.Models.User;

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

    public async Task<bool> UpdateMessage(Message message)
    {
        var response = await _messageContainer.PatchItemAsync<Message>(
            message.Id.ToString(),
            new PartitionKey(message.Id.ToString()),
            new[]
            {
                PatchOperation.Replace("/description", message.Content)
            });


        return response.StatusCode == HttpStatusCode.OK;

    }

    public async Task<bool> DeleteMessage(Guid id)
    {
         var response = await _messageContainer.DeleteItemAsync<Message>(id.ToString(), new PartitionKey(id.ToString()));
         return response.StatusCode == HttpStatusCode.OK;
    }

    private async Task<bool> UpsertMessage(Message message)
    {
        try
        {
            var response = await _messageContainer.UpsertItemAsync(message, new PartitionKey(message.PartitionKey));
            return response.StatusCode == HttpStatusCode.Created;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<bool> UpdateMessageUserAvatar(User user)
    {
        var messages = new List<Message>();
        try
        {
            var queryDefinition = new QueryDefinition("Select * from r where r.partionKey = @partitionKey")
                .WithParameter("@partitionKey", user.Email);

            var queryFromMessagesContainer =
                _messageContainer.GetItemQueryIterator<Message>(queryDefinition);

            while (queryFromMessagesContainer.HasMoreResults)
            {
                var response = await queryFromMessagesContainer.ReadNextAsync();
                var ru = response.RequestCharge;
                messages.AddRange(response.ToList());
            }

            foreach (var message in messages.ToList())
            {
                message.User.Avatar = user.Avatar;
                var response = await _messageContainer.UpsertItemAsync(message, new PartitionKey(message.PartitionKey));
            }

            /* An Example using a stored procedure
            var obj = new dynamic[] { user.Avatar };
            _ = await _messageContainer.Scripts.ExecuteStoredProcedureAsync<string>("updateUserAvatar", new PartitionKey(user.Email), obj);
            */

            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }
}
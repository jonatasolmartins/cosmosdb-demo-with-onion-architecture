using System.Net;
using Azure.Storage.Queues;
using ChatRoom.Core.Domain.Abstractions.Services;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using User = ChatRoom.Core.Domain.Models.User;

namespace ChatRoom.Core.Application.Services.Implementations;

public class UserService : IUserService
{
    private readonly Container _userContainer;
    private readonly QueueClient _queueClient;
    
    public UserService(CosmosClient cosmosClient, IConfiguration configuration, QueueClient queue)
    {
        var databaseName = configuration.GetSection("ConnectionStrings:DatabaseName").Value;
        _userContainer = cosmosClient.GetContainer(databaseName, "User");
        _queueClient = queue;
    }
    
    public async Task<bool> Create(User user)
    {
        var response = await _userContainer.UpsertItemAsync<User>(user, new PartitionKey(user.Email.ToString()));
        return response.StatusCode == HttpStatusCode.Created;
    }

    public async Task<bool> UpdateAvatar(User user)
    {
        var response = await _userContainer.PatchItemAsync<User>(
            user.Id.ToString(),
            new PartitionKey(user.Email),
            new[]
            {
                PatchOperation.Replace("/avatar", user.Avatar)
            });


        if (response.StatusCode != HttpStatusCode.OK) return false;
        
        var data = JsonConvert.SerializeObject(user);
        await _queueClient.SendMessageAsync(data);
        return true;
    }

    public async Task<bool> Delete(Guid id)
    {
        return await Task.FromResult(true);
    }
}
using System.Net;
using ChatRoom.Core.Domain.Abstractions.Repositories;
using ChatRoom.Core.Domain.Abstractions.Services;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using User = ChatRoom.Core.Domain.Models.User;

namespace ChatRoom.Core.Application.Services.Implementations;

public class UserService : IUserService
{
    private readonly Container _userContainer;

    //private readonly QueueClient _queueClient;
    private readonly IMultimediaRepository _multimediaRepository;

    public UserService(CosmosClient cosmosClient, IConfiguration configuration,
        IMultimediaRepository multimediaRepository)
    {
        var databaseName = configuration.GetSection("ConnectionStrings:DatabaseName").Value;
        _userContainer = cosmosClient.GetContainer(databaseName, "User");
        //_queueClient = queue;
        _multimediaRepository = multimediaRepository;
    }
    
    public async Task<bool> Create(User user)
    {
        var imageUrl = await _multimediaRepository.UploadImage(user.Avatar, "useravatar", user.Email);
        user.Avatar = imageUrl;
        var response = await _userContainer.UpsertItemAsync<User>(user, new PartitionKey(user.Email.ToString()));
        return response.StatusCode == HttpStatusCode.Created;
    }

    public async Task<bool> UpdateAvatar(User user)
    {
        try
        {
            var url = await _multimediaRepository.UploadImage(user.Avatar, "useravatar", user.Email);
            //var data = JsonConvert.SerializeObject(user);
            //await _queueClient.SendMessageAsync(data);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
       
        return true;
    }

    public async Task<bool> Delete(Guid id)
    {
        return await Task.FromResult(true);
    }
}
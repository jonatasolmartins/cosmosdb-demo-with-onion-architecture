using ChatRoom.Core.Domain.Abstractions.Repositories;
using ChatRoom.Core.Domain.Models;
using ChatRoom.Infrastructure.Database.AppSettings;
using Microsoft.Azure.Cosmos;

namespace ChatRoom.Infrastructure.Database.Implementations;

public class Repository  : IRepository
{
    private readonly Container _roomContainer;

    public Repository(CosmosClient cosmosClient, CosmoDbSettings cosmoDbSettings)
    {
        _roomContainer = cosmosClient.GetContainer(cosmoDbSettings.DatabaseName, "Room");
    }

    public async Task<List<Room>> GetAll()
    {
        var rooms = new List<Room>();

        var queryString = "Select r.id, r.name, udf.convertDate(r.dateCreated) as dateCreated, r.chats from r";

        var queryFromRoomsContainer = _roomContainer.GetItemQueryIterator<Room>(new QueryDefinition(queryString));
        while (queryFromRoomsContainer.HasMoreResults)
        {
            var response = await queryFromRoomsContainer.ReadNextAsync();
            var ru = response.RequestCharge;
            rooms.AddRange(response.ToList());
        }

        return rooms;
    }

    public async Task<Room> GetById(Guid id)
    {
        var response = await _roomContainer.ReadItemAsync<Room>(id.ToString(), new PartitionKey(id.ToString()));
        var ru = response.RequestCharge;
        return response.Resource;
    }

    public async Task<bool> Create(Room newRoom)
    {
        await _roomContainer.UpsertItemAsync<Room>(newRoom, new PartitionKey(newRoom.Id.ToString()));
        return true;
    }
    
    public async Task<bool> Delete(Guid id)
    {
        return await Task.FromResult(true);
    }

    public async Task<string> UpdateRecentMessage(Message message)
    {
        var obj = new dynamic[] { message.ChatId.ToString(), message };
        return await _roomContainer.Scripts.ExecuteStoredProcedureAsync<string>("updateMessage", new PartitionKey(message.RoomId.ToString()), obj);
    }
    
}
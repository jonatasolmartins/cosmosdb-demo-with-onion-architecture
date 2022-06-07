using ChatRoom.Core.Domain.Abstractions.Repositories;
using ChatRoom.Core.Domain.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;

namespace ChatRoom.Infrastructure.Database.Implementations;

public class Repository  : IRepository
{
    private readonly Container _roomContainer;

    public Repository(CosmosClient cosmosClient, IConfiguration configuration)
    {
        var configurationSection = configuration.GetSection("CosmosConnection");
        string databaseName = configurationSection.GetSection("DatabaseName").Value;
        _roomContainer = cosmosClient.GetContainer(databaseName, "Room");
    }

    public async Task<List<Room>> GetAll()
    {
        var rooms = new List<Room>();
        try
        {
            var queryString = $"Select r.id, r.name, udf.convertDate(r.dateCreated) as dateCreated, r.chats from r";

            var queryFromRoomsContainer = _roomContainer.GetItemQueryIterator<Room>(new QueryDefinition(queryString));
            while (queryFromRoomsContainer.HasMoreResults)
            {
                var response = await queryFromRoomsContainer.ReadNextAsync();
                var ru = response.RequestCharge;
                rooms.AddRange(response.ToList());
            }


            return rooms;
        }
        catch (Exception e)
        {
            return null;
        }
    }

    public async Task<Room> GetById(Guid id)
    {

        try
        {
           
            ItemResponse<Room> response = await this._roomContainer.ReadItemAsync<Room>(id.ToString(), new PartitionKey(id.ToString()));
            var ru = response.RequestCharge;
            return response.Resource;
        }
        catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
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
    
}
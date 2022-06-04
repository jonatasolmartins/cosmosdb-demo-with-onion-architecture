using ChatRoom.Core.Domain.Abstractions.Repositories;
using ChatRoom.Core.Domain.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;

namespace ChatRoom.Infrastructure.Database.Implementations;

public class RoomRepository : IRoomRepository
{
    private readonly Container _roomContainer;

    public RoomRepository(CosmosClient cosmosClient, IConfiguration configuration)
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
            var queryString = $"Select top 100 * from Room";

            var queryFromRoomsContainer = _roomContainer.GetItemQueryIterator<Room>(new QueryDefinition(queryString));
            while (queryFromRoomsContainer.HasMoreResults)
            {
                var response = await queryFromRoomsContainer.ReadNextAsync();
                //var ru = response.RequestCharge;
                rooms.AddRange(response.ToList());
            }


            return rooms;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return null;
        }
    }
}
using ChatRoom.Core.Domain.Models;

namespace ChatRoom.Core.Domain.Abstractions.Repositories;

public interface IRoomRepository
{
    Task<List<Room>> GetAll();
}
using ChatRoom.Core.Domain.Models;

namespace ChatRoom.Core.Domain.Abstractions.Repositories;

public interface IRepository
{
    Task<List<Room>> GetAll();
    Task<Room> GetById(Guid id);
    Task<bool> Create(Room model);
    Task<bool> Delete(Guid id);
    Task<string> UpdateRecentMessage(Message message);
}
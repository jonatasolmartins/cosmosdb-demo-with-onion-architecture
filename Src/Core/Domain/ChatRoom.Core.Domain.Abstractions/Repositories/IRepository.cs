using ChatRoom.Core.Domain.Models;
using ChatRoom.SharedKernel.Interfaces;

namespace ChatRoom.Core.Domain.Abstractions.Repositories;

public interface IRepository  :  IAggregateRoot
{
    Task<List<Room>> GetAll();
    Task<Room> GetById(Guid id);
    Task<bool> Create(Room model);
    Task<bool> Delete(Guid id);
}
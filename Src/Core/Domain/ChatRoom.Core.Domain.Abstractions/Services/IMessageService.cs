using ChatRoom.Core.Domain.Models;

namespace ChatRoom.Core.Domain.Abstractions.Services;

public interface IMessageService
{
    Task<bool> CreateMessage(Message newMessage, Guid roomId);
    Task<bool> UpdateMessage(Message message, Guid roomId);
    Task<bool> DeleteMessage(Guid id);
}
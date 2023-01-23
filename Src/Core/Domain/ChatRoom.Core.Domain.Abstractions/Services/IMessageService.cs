using ChatRoom.Core.Domain.Models;

namespace ChatRoom.Core.Domain.Abstractions.Services;

public interface IMessageService
{
    Task<bool> CreateMessage(Message newMessage);
    Task<bool> UpdateMessage(Message message);
    Task<bool> DeleteMessage(Guid id);
    Task<bool> UpdateMessageUserAvatar(User user);
}
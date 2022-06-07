using ChatRoom.Core.Domain.Abstractions.Services;
using ChatRoom.Core.Domain.Models;

namespace ChatRoom.Core.Application.Services.Implementations;

public class MessageService : IMessageService
{
    public MessageService()
    {
        
    }
    public async Task<bool> CreateMessage(Message newMessage)
    {
        return await Task.FromResult(true);
    }

    public async Task<bool> UpdateMessage(Message message)
    {
        return await Task.FromResult(true);
    }

    public async Task<bool> DeleteMessage(Guid id)
    {
        return await Task.FromResult(true);
    }
}
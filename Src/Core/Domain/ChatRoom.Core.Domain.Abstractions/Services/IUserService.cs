using ChatRoom.Core.Domain.Models;

namespace ChatRoom.Core.Domain.Abstractions.Services;

public interface IUserService
{
    Task<bool> Create(User user);
    Task<bool> UpdateAvatar(User user);
    Task<bool> Delete(Guid id);
}
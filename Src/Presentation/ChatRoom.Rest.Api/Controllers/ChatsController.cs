using ChatRoom.Core.Domain.Models;
using ChatRoom.Rest.Api.DataTransferObject;
using Microsoft.AspNetCore.Mvc;

namespace ChatRoom.Rest.Api.Controllers;

[Route("api/Rooms/[controller]")]
[ApiController]
public class ChatsController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(ChatToCreate newChat)
    {
        var chat = new Chat()
        {
            Id = Guid.NewGuid(),
            Name = newChat.Name,
            DateCreated = DateTime.UtcNow,
            Messages = new List<Message>()
        };
        var result = await Task.FromResult(chat);
        return Ok(result);
    }
}
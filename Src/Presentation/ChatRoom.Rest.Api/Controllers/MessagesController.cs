using ChatRoom.Core.Domain.Abstractions.Services;
using ChatRoom.Core.Domain.Models;
using ChatRoom.Rest.Api.DataTransferObject;
using Microsoft.AspNetCore.Mvc;

namespace ChatRoom.Rest.Api.Controllers;

[Route("api/Rooms/[controller]")]
[ApiController]
public class MessagesController : ControllerBase
{
    private readonly IMessageService _messageService;

    public MessagesController(IMessageService messageService)
    {
        _messageService = messageService;
    }

    [HttpPost]
    public async Task<IActionResult> Create(MessageToCreate newMessage)
    {
        var message = new Message()
        {
            Id = Guid.NewGuid(),
            ChatId = newMessage.ChatId,
            DateCreated = DateTime.UtcNow,
            Description = newMessage.Content
        };
        
        var result = await _messageService.CreateMessage(message, newMessage.RoomId);
        if (!result) return BadRequest();
        
        return Ok();
    }
}
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
        //TODO use auto Mapper instead
        var message = new Message()
        {
            Id = Guid.NewGuid(),
            ChatId = newMessage.ChatId,
            RoomId = newMessage.RoomId,
            DateCreated = DateTime.UtcNow,
            Content = newMessage.Content,
            PartitionKey = newMessage.User.Email,
            User = new User()
            {
                Id = newMessage.User.Id,
                Name = newMessage.User.Name,
                Email = newMessage.User.Email,
                Avatar = newMessage.User.Avatar
            }
        };
        
        var result = await _messageService.CreateMessage(message);
        if (!result) return BadRequest();

        return Ok();
    }

    [HttpPut]
    public async Task<IActionResult> Update(MessageToCreate newMessage)
    {
        var message = new Message
        {
            Id = newMessage.Id,
            Content = newMessage.Content,
            PartitionKey = newMessage.User.Email
        };

        var result = await _messageService.UpdateMessage(message);
        if (!result) return BadRequest();
        
        return Ok();
    }
}

using ChatRoom.Core.Domain.Models;
using ChatRoom.Rest.Api.DataTransferObject;
using Microsoft.AspNetCore.Mvc;

namespace ChatRoom.Rest.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MessagesController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(MessageToCreate newMessage)
    {
        var message = new Message()
        {
            Id = Guid.NewGuid(),
            Description = newMessage.Description
        };
        var result = await Task.FromResult(message);
        return Ok(result);
    }
}
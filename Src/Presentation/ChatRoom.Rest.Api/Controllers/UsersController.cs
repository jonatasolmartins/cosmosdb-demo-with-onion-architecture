using ChatRoom.Core.Domain.Abstractions.Services;
using ChatRoom.Core.Domain.Models;
using ChatRoom.Rest.Api.DataTransferObject;
using Microsoft.AspNetCore.Mvc;

namespace ChatRoom.Rest.Api.Controllers;

[ApiController]
[Route("api/Rooms/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    public UsersController(IUserService userService)
    {
        _userService = userService;
    }
    [HttpPost]
    public async Task<IActionResult> Create(UserMessage userMessage)
    {
        var user = new User()
        {
            Id = Guid.NewGuid(),
            Name = userMessage.Name,
            Email = userMessage.Email,
            Avatar = userMessage.Avatar
        };

        var result = await _userService.Create(user);
        if (!result) return BadRequest();

        return Ok();
    }

    [HttpPut("UpdateAvatar")]
    public async Task<IActionResult> UpdateAvatar(UserMessage userMessage)
    {
        var user = new User
        {
            Id = userMessage.Id,
            Name = userMessage.Name,
            Email = userMessage.Email,
            Avatar = userMessage.Avatar
        };

        var result = await _userService.UpdateAvatar(user);
        if (!result) return BadRequest();
        
        return Ok();
    }
}
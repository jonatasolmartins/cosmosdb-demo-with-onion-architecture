using ChatRoom.Core.Domain.Abstractions.Repositories;
using ChatRoom.Core.Domain.Models;
using ChatRoom.Rest.Api.DataTransferObject;
using Microsoft.AspNetCore.Mvc;

namespace ChatRoom.Rest.Api.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class RoomsController : ControllerBase
{
    private readonly IRepository _roomRepository;
    public RoomsController(IRepository repository)
    {
        _roomRepository = repository;
    }
    
    [HttpGet]
    [Produces(typeof(List<Room>))]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var result = await _roomRepository.GetAll();
            if (result == null) NoContent();
            
            return Ok(result);
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
      
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _roomRepository.GetById(id);
        if (result == null) NotFound("Room not found");
            
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(RoomForCreate newRoomForCreate)
    {
        var room = new Room()
        {
            Id = Guid.NewGuid(),
            Name = newRoomForCreate.Name,
            DateCreated = DateTime.UtcNow.ToString(),
            Chats = new List<Chat>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = newRoomForCreate.Name,
                    DateCreated = DateTime.Now,
                    Messages = new List<Message>()
                }
            },
        };
        var result = await _roomRepository.Create(room);

        if (!result) return BadRequest();

        return StatusCode(201);
    }
}
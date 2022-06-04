using ChatRoom.Core.Domain.Abstractions.Repositories;
using ChatRoom.Core.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace ChatRoom.Rest.Api.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class RoomsController : ControllerBase
{
    private readonly IRoomRepository _roomRepository;
    public RoomsController(IRoomRepository roomRepository)
    {
        _roomRepository = roomRepository;
    }

    [HttpGet]
    [Produces(typeof(List<Room>))]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var result = await _roomRepository.GetAll();
            if (result == null) throw new ArgumentException();
            
            return Ok(result);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(statusCode: 500);
        }
      
    }
}
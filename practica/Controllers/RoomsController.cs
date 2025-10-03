using Microsoft.AspNetCore.Mvc;
namespace practica.Controllers
{
    [ApiController]
    [Route("api/{controller}")]
    public class RoomsController : ControllerBase
    {
        private static readonly List<Room> _rooms = new()
        {
            new Room {Id=Guid.NewGuid(),RoomNumber="1111",RoomType="Simple",PricePerNight=120,Avaliable=true},
            new Room {Id=Guid.NewGuid(),RoomNumber="2222",RoomType="Double",PricePerNight=160,Avaliable=false},
            new Room {Id=Guid.NewGuid(),RoomNumber="3333",RoomType="suite",PricePerNight=220,Avaliable=true}
        };
        [HttpGet("{id:guid}")]
        public IActionResult GetOne(Guid id)
        {
            var room = _rooms.FirstOrDefault(r=>r.Id == id);
            return room is null ? NotFound(new {error = "Room not found", status = 404})
                : Ok(room);
        }
    }
}

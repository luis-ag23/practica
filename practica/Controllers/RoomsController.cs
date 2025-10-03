using Microsoft.AspNetCore.Mvc;
using System.Net.NetworkInformation;
using System.Numerics;
using System.Reflection;
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
            var room = _rooms.FirstOrDefault(r => r.Id == id);
            return room is null ? NotFound(new { error = "Room not found", status = 404 })
                : Ok(room);
        }
        [HttpPost]
        public IActionResult Create([FromBody] CreateRoomDto dto)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);
            var room = new Room
            {
                Id = Guid.NewGuid(),
                RoomNumber = dto.RoomNumber.Trim(),
                RoomType = dto.RoomType.Trim(),
                PricePerNight = dto.PricePerNight,
                Avaliable = dto.Avaliable
            };
            _rooms.Add(room);
            return CreatedAtAction(nameof(GetOne), new { id = room.Id }, room);
        }
        [HttpPut("{id:guid}")]
        public IActionResult Update([FromBody] UpdateRoomDto dto, Guid id)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);
            var index = _rooms.FindIndex(r => r.Id == id);
            if (index == -1)
                return NotFound(new { error = "Room not found", status = 404 });
            var update = new Room
            {
                Id = id,
                RoomNumber = dto.RoomNumber.Trim(),
                RoomType = dto.RoomType.Trim(),
                PricePerNight= dto.PricePerNight,
                Avaliable = dto.Avaliable
            };
            _rooms[index] = update;
            return Ok(update);
        }
        [HttpDelete("{id:guid}")]
        public IActionResult Delete(Guid id)
        {
            var removed = _rooms.RemoveAll(r => r.Id == id);
            return removed == 0 ? NotFound(new { error = "Room not found", status = 404 })
                : NoContent();
        }
        private static (int page,int limit) NormalizePage (int? page,int? limit)
        {
            var p = page.GetValueOrDefault(1); if (p < 1) p = 1;
            var l = limit.GetValueOrDefault(10); if (l < 1) l = 1; if (l > 100) l = 100;
            return (p, l);
        }
        private static IEnumerable<T> OrderByProp<T>(IEnumerable<T> src,string? sort,string? order)
        {
            if (string.IsNullOrWhiteSpace(sort)) return src;
            var prop = typeof(T).GetProperty(sort, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (prop is null) return src;
            return string.Equals(order, "desc", StringComparison.OrdinalIgnoreCase)
                ? src.OrderByDescending(x => prop.GetValue(x))
                : src.OrderBy(x => prop.GetValue(x));
        }
        [HttpGet]
        public IActionResult GetAll([FromQuery] int? page,
                                    [FromQuery] int? limit,
                                    [FromQuery] string? sort,
                                    [FromQuery] string? order,
                                    [FromQuery] string? q
            )
        {
            var (p, l) = NormalizePage(page, limit);
            IEnumerable<Room> query = _rooms;
            if (!string.IsNullOrWhiteSpace(q))
            {
                query = query.Where(r =>
                r.RoomNumber.Contains(q, StringComparison.OrdinalIgnoreCase) ||
                r.RoomType.Contains(q, StringComparison.OrdinalIgnoreCase));
            }
            query = OrderByProp(query, sort, order);
            var total = query.Count();
            var data = query.Skip((p - 1) * l).Take(l).ToList();
            return Ok(new
            {
                data,
                meta = new { page = p, limit = l, total }
            });
        }
    }

}

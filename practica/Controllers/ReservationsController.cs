using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace practica.Controllers
{
    [ApiController]
    [Route("api/{controller}")]
    public class ReservationsController : ControllerBase
    {
        private static readonly List<Reservation> _reservations = new()
        {
            new Reservation{Id=Guid.NewGuid(),RoomId=Guid.NewGuid(),CustomerId=Guid.NewGuid(),CheckInDate=DateTime.Parse("12-02-2022"),CheckOutDate=DateTime.Parse("13-02-2022"),Status="confirmed"},
            new Reservation{Id=Guid.NewGuid(),RoomId=Guid.NewGuid(),CustomerId=Guid.NewGuid(),CheckInDate=DateTime.Parse("13-03-2022"),CheckOutDate=DateTime.Parse("14-03-2022"),Status="pending"},
            new Reservation{Id=Guid.NewGuid(),RoomId=Guid.NewGuid(),CustomerId=Guid.NewGuid(),CheckInDate=DateTime.Parse("14-04-2022"),CheckOutDate=DateTime.Parse("15-04-2022"),Status="canceled"},
            new Reservation{Id=Guid.NewGuid(),RoomId=Guid.NewGuid(),CustomerId=Guid.NewGuid(),CheckInDate=DateTime.Parse("15-05-2022"),CheckOutDate=DateTime.Parse("16-05-2022"),Status="confirmed"}
        };
        [HttpGet("{id:guid}")]
        public IActionResult GetOne(Guid id)
        {
            var reservation = _reservations.FirstOrDefault(r => r.Id == id);
            return reservation is null ? NotFound(new { error = "Reservation not found" })
                : Ok(reservation);
        }
        [HttpPost]
        public IActionResult Create([FromBody] CreateReservationDto dto)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);
            var reservation = new Reservation
            {
                Id = Guid.NewGuid(),
                RoomId = Guid.NewGuid(),
                CustomerId = Guid.NewGuid(),
                CheckInDate = dto.CheckInDate,
                CheckOutDate = dto.CheckOutDate,
                Status = dto.Status.Trim()
            };
            _reservations.Add(reservation);
            return CreatedAtAction(nameof(GetOne), new { id = reservation.Id }, reservation);
        }
        [HttpPut("{id:guid}")]
        public IActionResult Update(Guid id, [FromBody] UpdateReservationDto dto)
        {
            if(!ModelState.IsValid) return ValidationProblem(ModelState);
            var index = _reservations.FindIndex(r => r.Id == id);
            if (index == -1)
                return NotFound(new { error = "reservation not found", status = 404 });
            var upadate = new Reservation
            {
                Id = id,
                RoomId = Guid.NewGuid(),
                CustomerId = Guid.NewGuid(),
                CheckInDate = dto.CheckInDate,
                CheckOutDate = dto.CheckOutDate,
                Status = dto.Status.Trim()
            };
            _reservations[index] = upadate;
            return Ok(upadate);
        }
        [HttpDelete("{id:guid}")]
        public IActionResult Delete(Guid id)
        {
            var removed = _reservations.RemoveAll(r => r.Id == id);
            return removed == 0 ? NotFound(new { error = "reservation not found", status = 404 })
                : NoContent();
        }
        
        private static (int page, int limit) NormalizePage(int? page,int? limit)
        {
            var p = page.GetValueOrDefault(1); if (p < 1) p = 1;
            var l = limit.GetValueOrDefault(10); if (l < 1) l = 1; if (l>100) l = 100;
            return (p,l);
        }
        private IEnumerable<T> OrderByProp<T>(IEnumerable<T> src,string? sort,string? order)
        {
            if (string.IsNullOrWhiteSpace(sort)) return src;
            var prop = typeof(T).GetProperty(sort, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (prop == null) return src;
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
            IEnumerable<Reservation> query = _reservations;
            if(!string.IsNullOrWhiteSpace(q))
            {
                query = query.Where(r =>
                r.Status.Contains(q, StringComparison.OrdinalIgnoreCase));
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

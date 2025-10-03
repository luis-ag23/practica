using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

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
            return reservation is null ? NotFound(new {error="Reservation not found"})
                :Ok(reservation);
        }

    }
}

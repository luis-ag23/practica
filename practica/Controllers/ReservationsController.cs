using Microsoft.AspNetCore.Mvc;

namespace practica.Controllers
{
    [ApiController]
    [Route("api/{controller}")]
    public class ReservationsController
    {
        private static readonly List<Reservation> _reservations = new()
        {
            new Reservation{
                Id=Guid.NewGuid(),RoomId=Guid.NewGuid(),CustomerId=Guid.NewGuid(),CheckInDate=DateTime.Parse("12-02-2022"),CheckOutDate=DateTime.Parse("13-02-2022"),
                Status="confirmed"},
        };
    }
}

using System.ComponentModel.DataAnnotations;
public class Reservation
{
    public Guid Id { get; set; }
    [Required]
    public Guid RoomId { get; set; }
    [Required]
    public Guid CustomerId { get; set; }
    [Required]
    public DateTime CheckInDate { get; set; }
    [Required]
    public DateTime CheckOutDate { get; set; }
    [Required]
    public string Status { get; set; } = string.Empty;
}
public record CreateReservationDto
{
    [Required]
    public DateTime CheckInDate { get; set; }
    [Required]
    public DateTime CheckOutDate { get; set; }
    [Required]
    public string Status { get; set; } = string.Empty;
}
public record UpdateReservationDto
{
    public required DateTime CheckInDate { get; set; }
    public required DateTime CheckOutDate { get; set; }
    public required string Status { get; set; }
}

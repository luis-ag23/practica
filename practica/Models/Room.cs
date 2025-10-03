using System.ComponentModel.DataAnnotations;
public class Room
{
    public Guid Id { get; set; }
    [Required]
    public string RoomNumber { get; set; } = string.Empty;
    [Required]
    public string RoomType { get; set; } = string.Empty;
    [Required]
    public decimal PricePerNight { get; set; }
    [Required]
    public bool Avaliable { get; set; }
}

public record CreateRoomDto
{
    [Required]
    public string RoomNumber { get; set;} = string.Empty;

    [Required]
    public string RoomType { get; set;} = string.Empty ;
    [Required]
    public decimal PricePerNight { get;set;}
    [Required]
    public bool Avaliable { get; set; }
}
public record UpdateRoomDto
{
    public required string RoomNumber { get; set;}
    public required string RoomType { get; set;}
    public decimal PricePerNight { get; set;}
    public required bool Avaliable { get; set;}
}
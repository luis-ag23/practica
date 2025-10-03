using System.ComponentModel.DataAnnotations;
public class Payment
{
    public Guid Id { get; set; }
    public Guid ReservationId { get; set; }
    [Required]
    public decimal Amount { get; set; }

    [Required]
    public DateTime PaymentDate { get; set; }
    [Required]
    public string PaymentStatus { get; set; } = string.Empty;
}

public record CreatePaymentDto
{
    [Required]
    public decimal Amount { get; set; }
    [Required]
    public DateTime PaymentDate { get; set; }
    [Required]
    public string PaymentStatus { get; set; } = string.Empty;
}

public record UpdatePaymentDto
{
    public required decimal Amount { get; set; }
    public required DateTime PaymentDate { get; set; }
    public required string PaymentStatus { get; set; }
}
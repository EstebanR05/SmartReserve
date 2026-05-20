namespace SistemaReservas.Web.Contracts.Reservations;

public class ReservationDto
{
    public int Id { get; set; }
    public int TouristSiteId { get; set; }
    public string SiteName { get; set; } = string.Empty;
    public DateOnly CheckInDate { get; set; }
    public DateOnly CheckOutDate { get; set; }
    public int Adults { get; set; }
    public int Children { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; }
}

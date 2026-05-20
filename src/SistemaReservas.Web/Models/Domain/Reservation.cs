namespace SistemaReservas.Web.Models.Domain;

public class Reservation
{
    public int Id { get; set; }
    public int TouristSiteId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string ContactFullName { get; set; } = string.Empty;
    public string ContactEmail { get; set; } = string.Empty;
    public string? ContactPhone { get; set; }
    public DateOnly CheckInDate { get; set; }
    public DateOnly CheckOutDate { get; set; }
    public int Adults { get; set; }
    public int Children { get; set; }
    public int TotalPeople => Adults + Children;
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = "Pending";
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public TouristSite? TouristSite { get; set; }
    public ICollection<ReservationUnit> ReservationUnits { get; set; } = new List<ReservationUnit>();
}

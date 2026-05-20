namespace SistemaReservas.Web.Contracts.Availability;

public class AvailabilitySearchRequest
{
    public int TouristSiteId { get; set; }
    public DateOnly CheckInDate { get; set; }
    public DateOnly CheckOutDate { get; set; }
    public int? People { get; set; }
}

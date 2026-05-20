namespace SistemaReservas.Web.Contracts.Reservations;

public class CreateReservationRequest
{
    public int TouristSiteId { get; set; }
    public DateOnly CheckInDate { get; set; }
    public DateOnly CheckOutDate { get; set; }
    public int Adults { get; set; }
    public int Children { get; set; }
    public string ContactFullName { get; set; } = string.Empty;
    public string ContactEmail { get; set; } = string.Empty;
    public string? ContactPhone { get; set; }
    public List<ReservationUnitRequest> Units { get; set; } = [];
}

public class ReservationUnitRequest
{
    public int AccommodationUnitId { get; set; }
    public int Quantity { get; set; } = 1;
    public int PeopleCount { get; set; }
    public decimal UnitPrice { get; set; }
}

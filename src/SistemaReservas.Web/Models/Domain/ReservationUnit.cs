namespace SistemaReservas.Web.Models.Domain;

public class ReservationUnit
{
    public int Id { get; set; }
    public int ReservationId { get; set; }
    public int AccommodationUnitId { get; set; }
    public int Quantity { get; set; } = 1;
    public int PeopleCount { get; set; }
    public decimal UnitPrice { get; set; }

    public Reservation? Reservation { get; set; }
    public AccommodationUnit? AccommodationUnit { get; set; }
}

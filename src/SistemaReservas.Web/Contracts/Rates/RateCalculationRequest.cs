namespace SistemaReservas.Web.Contracts.Rates;

public class RateCalculationRequest
{
    public int TouristSiteId { get; set; }
    public DateOnly ReferenceDate { get; set; }
    public int People { get; set; }
    public int AccommodationTypeId { get; set; }
    public int RoomCount { get; set; }
    public int Nights { get; set; }
}

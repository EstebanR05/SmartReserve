namespace SistemaReservas.Web.Contracts.Rates;

public class RateSearchRequest
{
    public int TouristSiteId { get; set; }
    public DateOnly ReferenceDate { get; set; }
    public int People { get; set; }
    public int AccommodationTypeId { get; set; }
    public int? AccommodationUnitId { get; set; }
}

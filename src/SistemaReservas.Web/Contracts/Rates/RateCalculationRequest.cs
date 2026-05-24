using System.ComponentModel.DataAnnotations;

namespace SistemaReservas.Web.Contracts.Rates;

public class RateCalculationRequest
{
    [Range(1, int.MaxValue)]
    public int TouristSiteId { get; set; }
    public DateOnly ReferenceDate { get; set; }

    [Range(1, int.MaxValue)]
    public int People { get; set; }

    [Range(1, int.MaxValue)]
    public int AccommodationTypeId { get; set; }

    [Range(1, int.MaxValue)]
    public int RoomCount { get; set; }

    [Range(1, int.MaxValue)]
    public int Nights { get; set; }
}

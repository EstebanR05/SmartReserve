using System.ComponentModel.DataAnnotations;

namespace SistemaReservas.Web.Contracts.Availability;

public class AvailabilitySearchRequest
{
    [Range(1, int.MaxValue)]
    public int TouristSiteId { get; set; }
    public DateOnly CheckInDate { get; set; }
    public DateOnly CheckOutDate { get; set; }

    [Range(1, int.MaxValue)]
    public int? People { get; set; }
}

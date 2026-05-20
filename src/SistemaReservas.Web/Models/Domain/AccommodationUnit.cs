namespace SistemaReservas.Web.Models.Domain;

public class AccommodationUnit
{
    public int Id { get; set; }
    public int TouristSiteId { get; set; }
    public int AccommodationTypeId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int MaxCapacity { get; set; }
    public int BedroomCount { get; set; }
    public int BathroomCount { get; set; }
    public bool HasKitchen { get; set; }
    public bool HasParking { get; set; }
    public bool IsActive { get; set; } = true;

    public TouristSite? TouristSite { get; set; }
    public AccommodationType? AccommodationType { get; set; }
    public ICollection<RatePlan> RatePlans { get; set; } = new List<RatePlan>();
    public ICollection<ReservationUnit> ReservationUnits { get; set; } = new List<ReservationUnit>();
}

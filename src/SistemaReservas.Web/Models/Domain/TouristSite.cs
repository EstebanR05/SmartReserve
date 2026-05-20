namespace SistemaReservas.Web.Models.Domain;

public class TouristSite
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string SiteType { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int MaxCapacity { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<AccommodationUnit> AccommodationUnits { get; set; } = new List<AccommodationUnit>();
    public ICollection<RatePlan> RatePlans { get; set; } = new List<RatePlan>();
    public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}

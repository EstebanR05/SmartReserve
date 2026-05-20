namespace SistemaReservas.Web.Models.Domain;

public class AccommodationType
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<AccommodationUnit> AccommodationUnits { get; set; } = new List<AccommodationUnit>();
    public ICollection<RatePlan> RatePlans { get; set; } = new List<RatePlan>();
}

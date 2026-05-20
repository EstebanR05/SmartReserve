namespace SistemaReservas.Web.Models.Domain;

public class RatePlan
{
    public int Id { get; set; }
    public int TouristSiteId { get; set; }
    public int AccommodationTypeId { get; set; }
    public int? AccommodationUnitId { get; set; }
    public int SeasonId { get; set; }
    public int MinPeople { get; set; }
    public int MaxPeople { get; set; }
    public decimal BasePrice { get; set; }
    public decimal AdditionalPersonPrice { get; set; }
    public string Currency { get; set; } = "COP";
    public string RateType { get; set; } = "Night";
    public bool IsActive { get; set; } = true;
    public string? Notes { get; set; }

    public TouristSite? TouristSite { get; set; }
    public AccommodationType? AccommodationType { get; set; }
    public AccommodationUnit? AccommodationUnit { get; set; }
    public Season? Season { get; set; }
}

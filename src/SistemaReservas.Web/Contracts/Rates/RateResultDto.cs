namespace SistemaReservas.Web.Contracts.Rates;

public class RateResultDto
{
    public int RatePlanId { get; set; }
    public string SiteName { get; set; } = string.Empty;
    public string SeasonName { get; set; } = string.Empty;
    public string AccommodationType { get; set; } = string.Empty;
    public string? UnitName { get; set; }
    public int MinPeople { get; set; }
    public int MaxPeople { get; set; }
    public decimal BasePrice { get; set; }
    public decimal AdditionalPersonPrice { get; set; }
}

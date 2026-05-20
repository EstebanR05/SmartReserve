namespace SistemaReservas.Web.Models.Domain;

public class Season
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public bool IsHighSeason { get; set; }
    public bool IsSpecialRate { get; set; }
    public string? Notes { get; set; }

    public ICollection<RatePlan> RatePlans { get; set; } = new List<RatePlan>();
}

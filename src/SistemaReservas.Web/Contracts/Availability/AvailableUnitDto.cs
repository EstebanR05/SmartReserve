namespace SistemaReservas.Web.Contracts.Availability;

public class AvailableUnitDto
{
    public int AccommodationUnitId { get; set; }
    public string UnitCode { get; set; } = string.Empty;
    public string UnitName { get; set; } = string.Empty;
    public int MaxCapacity { get; set; }
    public int Bedrooms { get; set; }
    public string AccommodationType { get; set; } = string.Empty;
}

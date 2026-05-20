namespace SistemaReservas.Web.Models.Queries;

public class RateCalculationQueryResult
{
    public decimal SubtotalPerNight { get; set; }
    public decimal TotalAmount { get; set; }
    public int Nights { get; set; }
    public int RoomCount { get; set; }
}

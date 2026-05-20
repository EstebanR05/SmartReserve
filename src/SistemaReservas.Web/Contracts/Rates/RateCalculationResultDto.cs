namespace SistemaReservas.Web.Contracts.Rates;

public class RateCalculationResultDto
{
    public decimal SubtotalPerNight { get; set; }
    public decimal TotalAmount { get; set; }
    public int Nights { get; set; }
    public int RoomCount { get; set; }
}

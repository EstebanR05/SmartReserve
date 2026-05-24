using System.ComponentModel.DataAnnotations;

namespace SistemaReservas.Web.Contracts.Reservations;

public class CreateReservationRequest
{
    [Range(1, int.MaxValue)]
    public int TouristSiteId { get; set; }
    public DateOnly CheckInDate { get; set; }
    public DateOnly CheckOutDate { get; set; }

    [Range(0, int.MaxValue)]
    public int Adults { get; set; }

    [Range(0, int.MaxValue)]
    public int Children { get; set; }

    [Required]
    [MaxLength(255)]
    public string ContactFullName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string ContactEmail { get; set; } = string.Empty;

    [Phone]
    public string? ContactPhone { get; set; }

    [MinLength(1)]
    public List<ReservationUnitRequest> Units { get; set; } = [];
}

public class ReservationUnitRequest
{
    [Range(1, int.MaxValue)]
    public int AccommodationUnitId { get; set; }

    [Range(1, int.MaxValue)]
    public int Quantity { get; set; } = 1;

    [Range(1, int.MaxValue)]
    public int PeopleCount { get; set; }

    [Range(typeof(decimal), "0.01", "79228162514264337593543950335")]
    public decimal UnitPrice { get; set; }
}

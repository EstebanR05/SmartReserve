using SistemaReservas.Web.Contracts.Reservations;

namespace SistemaReservas.Web.Services.Interfaces;

public interface IReservationService
{
    Task<ReservationDto> CreateAsync(string userId, CreateReservationRequest request, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<ReservationDto>> GetMineAsync(string userId, CancellationToken cancellationToken);
    Task<bool> CancelAsync(string userId, int reservationId, CancellationToken cancellationToken);
}

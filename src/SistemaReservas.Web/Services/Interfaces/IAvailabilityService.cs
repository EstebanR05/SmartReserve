using SistemaReservas.Web.Contracts.Availability;

namespace SistemaReservas.Web.Services.Interfaces;

public interface IAvailabilityService
{
    Task<IReadOnlyCollection<AvailableUnitDto>> SearchAsync(AvailabilitySearchRequest request, CancellationToken cancellationToken);
}

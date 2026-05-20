using SistemaReservas.Web.Contracts.Rates;

namespace SistemaReservas.Web.Services.Interfaces;

public interface IRateService
{
    Task<IReadOnlyCollection<RateResultDto>> SearchRatesAsync(RateSearchRequest request, CancellationToken cancellationToken);
    Task<RateCalculationResultDto> CalculateAsync(RateCalculationRequest request, CancellationToken cancellationToken);
}

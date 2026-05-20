using Microsoft.EntityFrameworkCore;
using SistemaReservas.Web.Contracts.Rates;
using SistemaReservas.Web.Data;
using SistemaReservas.Web.Services.Interfaces;

namespace SistemaReservas.Web.Services.Implementations;

public class RateService : IRateService
{
    private readonly ApplicationDbContext _dbContext;

    public RateService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyCollection<RateResultDto>> SearchRatesAsync(
        RateSearchRequest request,
        CancellationToken cancellationToken
    )
    {
        var results = await _dbContext.RateQueryResults
            .FromSqlInterpolated($"EXEC dbo.sp_GetRatesBySiteSeasonPeopleAccommodation @TouristSiteId={request.TouristSiteId}, @ReferenceDate={request.ReferenceDate}, @People={request.People}, @AccommodationTypeId={request.AccommodationTypeId}, @AccommodationUnitId={request.AccommodationUnitId}")
            .ToListAsync(cancellationToken);

        return results.Select(x => new RateResultDto
        {
            RatePlanId = x.RatePlanId,
            SiteName = x.SiteName,
            SeasonName = x.SeasonName,
            AccommodationType = x.AccommodationType,
            UnitName = x.UnitName,
            MinPeople = x.MinPeople,
            MaxPeople = x.MaxPeople,
            BasePrice = x.BasePrice,
            AdditionalPersonPrice = x.AdditionalPersonPrice
        }).ToList();
    }

    public async Task<RateCalculationResultDto> CalculateAsync(
        RateCalculationRequest request,
        CancellationToken cancellationToken
    )
    {
        if (request.Nights <= 0 || request.RoomCount <= 0 || request.People <= 0)
        {
            throw new ArgumentException("Nights, RoomCount and People must be greater than zero.");
        }

        var result = await _dbContext.RateCalculationQueryResults
            .FromSqlInterpolated($"EXEC dbo.sp_CalculateTotalRate @TouristSiteId={request.TouristSiteId}, @ReferenceDate={request.ReferenceDate}, @People={request.People}, @AccommodationTypeId={request.AccommodationTypeId}, @RoomCount={request.RoomCount}, @Nights={request.Nights}")
            .FirstOrDefaultAsync(cancellationToken);

        return result is null
            ? throw new InvalidOperationException("No rate configuration found for the provided criteria.")
            : new RateCalculationResultDto
            {
                SubtotalPerNight = result.SubtotalPerNight,
                TotalAmount = result.TotalAmount,
                Nights = result.Nights,
                RoomCount = result.RoomCount
            };
    }
}

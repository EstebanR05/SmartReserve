using Microsoft.EntityFrameworkCore;
using SistemaReservas.Web.Contracts.Availability;
using SistemaReservas.Web.Data;
using SistemaReservas.Web.Services.Interfaces;

namespace SistemaReservas.Web.Services.Implementations;

public class AvailabilityService : IAvailabilityService
{
    private readonly ApplicationDbContext _dbContext;

    public AvailabilityService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyCollection<AvailableUnitDto>> SearchAsync(
        AvailabilitySearchRequest request, 
        CancellationToken cancellationToken
    )
    {
        if (request.CheckOutDate <= request.CheckInDate)
        {
            throw new ArgumentException("CheckOutDate must be greater than CheckInDate.");
        }

        var results = request.People.HasValue
            ? await _dbContext.AvailableUnitQueryResults
                .FromSqlInterpolated($"EXEC dbo.sp_FindAvailableUnitsByDateAndPeople @TouristSiteId={request.TouristSiteId}, @CheckInDate={request.CheckInDate}, @CheckOutDate={request.CheckOutDate}, @People={request.People.Value}")
                .ToListAsync(cancellationToken)
            : await _dbContext.AvailableUnitQueryResults
                .FromSqlInterpolated($"EXEC dbo.sp_FindAvailableUnitsByDateRange @TouristSiteId={request.TouristSiteId}, @CheckInDate={request.CheckInDate}, @CheckOutDate={request.CheckOutDate}")
                .ToListAsync(cancellationToken);

        return results.Select(x => new AvailableUnitDto
        {
            AccommodationUnitId = x.AccommodationUnitId,
            UnitCode = x.UnitCode,
            UnitName = x.UnitName,
            MaxCapacity = x.MaxCapacity,
            Bedrooms = x.Bedrooms,
            AccommodationType = x.AccommodationType
        }).ToList();
    }
}

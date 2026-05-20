using Microsoft.EntityFrameworkCore;
using SistemaReservas.Web.Contracts.Reservations;
using SistemaReservas.Web.Data;
using SistemaReservas.Web.Models.Domain;
using SistemaReservas.Web.Services.Interfaces;

namespace SistemaReservas.Web.Services.Implementations;

public class ReservationService : IReservationService
{
    private readonly ApplicationDbContext _dbContext;

    public ReservationService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ReservationDto> CreateAsync(
        string userId,
        CreateReservationRequest request,
        CancellationToken cancellationToken
    )
    {
        if (request.CheckOutDate <= request.CheckInDate)
        {
            throw new ArgumentException("CheckOutDate must be greater than CheckInDate.");
        }

        if (request.Units.Count == 0)
        {
            throw new ArgumentException("At least one accommodation unit is required.");
        }

        var reservation = new Reservation
        {
            TouristSiteId = request.TouristSiteId,
            UserId = userId,
            ContactFullName = request.ContactFullName,
            ContactEmail = request.ContactEmail,
            ContactPhone = request.ContactPhone,
            CheckInDate = request.CheckInDate,
            CheckOutDate = request.CheckOutDate,
            Adults = request.Adults,
            Children = request.Children,
            Status = "Confirmed"
        };

        reservation.TotalAmount = request.Units.Sum(x => x.UnitPrice * x.Quantity);

        foreach (var unit in request.Units)
        {
            reservation.ReservationUnits.Add(new ReservationUnit
            {
                AccommodationUnitId = unit.AccommodationUnitId,
                Quantity = unit.Quantity,
                PeopleCount = unit.PeopleCount,
                UnitPrice = unit.UnitPrice
            });
        }

        _dbContext.Reservations.Add(reservation);
        await _dbContext.SaveChangesAsync(cancellationToken);

        var siteName = await _dbContext.TouristSites
            .Where(x => x.Id == reservation.TouristSiteId)
            .Select(x => x.Name)
            .FirstAsync(cancellationToken);

        return new ReservationDto
        {
            Id = reservation.Id,
            TouristSiteId = reservation.TouristSiteId,
            SiteName = siteName,
            CheckInDate = reservation.CheckInDate,
            CheckOutDate = reservation.CheckOutDate,
            Adults = reservation.Adults,
            Children = reservation.Children,
            TotalAmount = reservation.TotalAmount,
            Status = reservation.Status,
            CreatedAtUtc = reservation.CreatedAtUtc
        };
    }

    public async Task<IReadOnlyCollection<ReservationDto>> GetMineAsync(
        string userId,
        CancellationToken cancellationToken
    )
    {
        return await _dbContext.Reservations
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.CreatedAtUtc)
            .Select(x => new ReservationDto
            {
                Id = x.Id,
                TouristSiteId = x.TouristSiteId,
                SiteName = x.TouristSite!.Name,
                CheckInDate = x.CheckInDate,
                CheckOutDate = x.CheckOutDate,
                Adults = x.Adults,
                Children = x.Children,
                TotalAmount = x.TotalAmount,
                Status = x.Status,
                CreatedAtUtc = x.CreatedAtUtc
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> CancelAsync(
        string userId, 
        int reservationId, 
        CancellationToken cancellationToken
    )
    {
        var reservation = await _dbContext.Reservations
            .FirstOrDefaultAsync(x => x.Id == reservationId && x.UserId == userId, cancellationToken);

        if (reservation is null)
        {
            return false;
        }

        reservation.Status = "Cancelled";
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}

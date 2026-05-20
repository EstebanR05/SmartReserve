using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaReservas.Web.Contracts.Reservations;
using SistemaReservas.Web.Services.Interfaces;

namespace SistemaReservas.Web.Controllers.Api;

[ApiController]
[Route("api/reservations")]
[Authorize]
public class ReservationsController : ControllerBase
{
    private readonly IReservationService _reservationService;

    public ReservationsController(IReservationService reservationService)
    {
        _reservationService = reservationService;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateReservationRequest request, CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }

        var reservation = await _reservationService.CreateAsync(userId, request, cancellationToken);
        return CreatedAtAction(nameof(GetMine), new { id = reservation.Id }, reservation);
    }

    [HttpGet("mine")]
    public async Task<IActionResult> GetMine(CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }

        var reservations = await _reservationService.GetMineAsync(userId, cancellationToken);
        return Ok(reservations);
    }

    [HttpDelete("{reservationId:int}")]
    public async Task<IActionResult> Cancel(int reservationId, CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }

        var cancelled = await _reservationService.CancelAsync(userId, reservationId, cancellationToken);
        return cancelled ? NoContent() : NotFound();
    }
}

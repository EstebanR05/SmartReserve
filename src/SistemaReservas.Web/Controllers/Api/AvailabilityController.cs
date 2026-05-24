using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaReservas.Web.Contracts.Availability;
using SistemaReservas.Web.Services.Interfaces;

namespace SistemaReservas.Web.Controllers.Api;

[ApiController]
[Route("api/availability")]
[Authorize]
public class AvailabilityController : ControllerBase
{
    private readonly IAvailabilityService _availabilityService;

    public AvailabilityController(IAvailabilityService availabilityService)
    {
        _availabilityService = availabilityService;
    }

    [HttpPost("search")]
    public async Task<IActionResult> Search([FromBody] AvailabilitySearchRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var results = await _availabilityService.SearchAsync(request, cancellationToken);
            return Ok(results);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }
}

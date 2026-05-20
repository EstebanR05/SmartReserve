using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaReservas.Web.Contracts.Rates;
using SistemaReservas.Web.Services.Interfaces;

namespace SistemaReservas.Web.Controllers.Api;

[ApiController]
[Route("api/rates")]
[Authorize]
public class RatesController : ControllerBase
{
    private readonly IRateService _rateService;

    public RatesController(IRateService rateService)
    {
        _rateService = rateService;
    }

    [HttpPost("search")]
    public async Task<IActionResult> Search([FromBody] RateSearchRequest request, CancellationToken cancellationToken)
    {
        var results = await _rateService.SearchRatesAsync(request, cancellationToken);
        return Ok(results);
    }

    [HttpPost("calculate")]
    public async Task<IActionResult> Calculate([FromBody] RateCalculationRequest request, CancellationToken cancellationToken)
    {
        var result = await _rateService.CalculateAsync(request, cancellationToken);
        return Ok(result);
    }
}

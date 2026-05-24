using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaReservas.Web.Data;
using SistemaReservas.Web.Models.Domain;

namespace SistemaReservas.Web.Controllers.Api;

[ApiController]
[Route("api/rate-plans")]
[Authorize]
public class RatePlansController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;

    public RatePlansController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var data = await _dbContext.RatePlans
            .AsNoTracking()
            .Include(x => x.TouristSite)
            .Include(x => x.AccommodationType)
            .Include(x => x.AccommodationUnit)
            .Include(x => x.Season)
            .OrderBy(x => x.TouristSiteId)
            .ThenBy(x => x.SeasonId)
            .ToListAsync(cancellationToken);

        return Ok(data);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var ratePlan = await _dbContext.RatePlans
            .AsNoTracking()
            .Include(x => x.TouristSite)
            .Include(x => x.AccommodationType)
            .Include(x => x.AccommodationUnit)
            .Include(x => x.Season)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return ratePlan is null ? NotFound() : Ok(ratePlan);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] RatePlan input, CancellationToken cancellationToken)
    {
        var validationError = ValidateRatePlan(input);
        if (validationError is not null)
        {
            return BadRequest(new { Message = validationError });
        }

        _dbContext.RatePlans.Add(input);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = input.Id }, input);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] RatePlan input, CancellationToken cancellationToken)
    {
        var validationError = ValidateRatePlan(input);
        if (validationError is not null)
        {
            return BadRequest(new { Message = validationError });
        }

        var ratePlan = await _dbContext.RatePlans.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (ratePlan is null)
        {
            return NotFound();
        }

        ratePlan.TouristSiteId = input.TouristSiteId;
        ratePlan.AccommodationTypeId = input.AccommodationTypeId;
        ratePlan.AccommodationUnitId = input.AccommodationUnitId;
        ratePlan.SeasonId = input.SeasonId;
        ratePlan.MinPeople = input.MinPeople;
        ratePlan.MaxPeople = input.MaxPeople;
        ratePlan.BasePrice = input.BasePrice;
        ratePlan.AdditionalPersonPrice = input.AdditionalPersonPrice;
        ratePlan.Currency = input.Currency;
        ratePlan.RateType = input.RateType;
        ratePlan.IsActive = input.IsActive;
        ratePlan.Notes = input.Notes;

        await _dbContext.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var ratePlan = await _dbContext.RatePlans.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (ratePlan is null)
        {
            return NotFound();
        }

        _dbContext.RatePlans.Remove(ratePlan);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    private static string? ValidateRatePlan(RatePlan input)
    {
        if (input.MinPeople <= 0 || input.MaxPeople <= 0)
        {
            return "MinPeople and MaxPeople must be greater than zero.";
        }

        if (input.MinPeople > input.MaxPeople)
        {
            return "MinPeople cannot be greater than MaxPeople.";
        }

        if (input.BasePrice <= 0)
        {
            return "BasePrice must be greater than zero.";
        }

        if (input.AdditionalPersonPrice < 0)
        {
            return "AdditionalPersonPrice cannot be negative.";
        }

        if (string.IsNullOrWhiteSpace(input.Currency))
        {
            return "Currency is required.";
        }

        if (string.IsNullOrWhiteSpace(input.RateType))
        {
            return "RateType is required.";
        }

        return null;
    }
}

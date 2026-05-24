using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaReservas.Web.Data;
using SistemaReservas.Web.Models.Domain;

namespace SistemaReservas.Web.Controllers.Api;

[ApiController]
[Route("api/seasons")]
[Authorize]
public class SeasonsController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;

    public SeasonsController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var data = await _dbContext.Seasons
            .AsNoTracking()
            .OrderBy(x => x.StartDate)
            .ToListAsync(cancellationToken);

        return Ok(data);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var season = await _dbContext.Seasons
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return season is null ? NotFound() : Ok(season);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Season input, CancellationToken cancellationToken)
    {
        if (input.EndDate < input.StartDate)
        {
            return BadRequest(new { Message = "EndDate must be greater than or equal to StartDate." });
        }

        _dbContext.Seasons.Add(input);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = input.Id }, input);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] Season input, CancellationToken cancellationToken)
    {
        if (input.EndDate < input.StartDate)
        {
            return BadRequest(new { Message = "EndDate must be greater than or equal to StartDate." });
        }

        var season = await _dbContext.Seasons.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (season is null)
        {
            return NotFound();
        }

        season.Name = input.Name;
        season.StartDate = input.StartDate;
        season.EndDate = input.EndDate;
        season.IsHighSeason = input.IsHighSeason;
        season.IsSpecialRate = input.IsSpecialRate;
        season.Notes = input.Notes;

        await _dbContext.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var season = await _dbContext.Seasons.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (season is null)
        {
            return NotFound();
        }

        _dbContext.Seasons.Remove(season);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return NoContent();
    }
}

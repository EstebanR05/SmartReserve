using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaReservas.Web.Data;
using SistemaReservas.Web.Models.Domain;

namespace SistemaReservas.Web.Controllers.Api;

[ApiController]
[Route("api/accommodation-units")]
[Authorize]
public class AccommodationUnitsController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;

    public AccommodationUnitsController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var data = await _dbContext.AccommodationUnits
            .AsNoTracking()
            .Include(x => x.TouristSite)
            .Include(x => x.AccommodationType)
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);

        return Ok(data);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var unit = await _dbContext.AccommodationUnits
            .AsNoTracking()
            .Include(x => x.TouristSite)
            .Include(x => x.AccommodationType)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return unit is null ? NotFound() : Ok(unit);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] AccommodationUnit input, CancellationToken cancellationToken)
    {
        _dbContext.AccommodationUnits.Add(input);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = input.Id }, input);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] AccommodationUnit input, CancellationToken cancellationToken)
    {
        var unit = await _dbContext.AccommodationUnits.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (unit is null)
        {
            return NotFound();
        }

        unit.TouristSiteId = input.TouristSiteId;
        unit.AccommodationTypeId = input.AccommodationTypeId;
        unit.Code = input.Code;
        unit.Name = input.Name;
        unit.Description = input.Description;
        unit.MaxCapacity = input.MaxCapacity;
        unit.BedroomCount = input.BedroomCount;
        unit.BathroomCount = input.BathroomCount;
        unit.HasKitchen = input.HasKitchen;
        unit.HasParking = input.HasParking;
        unit.IsActive = input.IsActive;

        await _dbContext.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var unit = await _dbContext.AccommodationUnits.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (unit is null)
        {
            return NotFound();
        }

        _dbContext.AccommodationUnits.Remove(unit);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return NoContent();
    }
}

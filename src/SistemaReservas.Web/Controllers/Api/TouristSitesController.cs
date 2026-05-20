using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaReservas.Web.Data;
using SistemaReservas.Web.Models.Domain;

namespace SistemaReservas.Web.Controllers.Api;

[ApiController]
[Route("api/tourist-sites")]
[Authorize]
public class TouristSitesController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;

    public TouristSitesController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var data = await _dbContext.TouristSites.AsNoTracking().OrderBy(x => x.Name).ToListAsync(cancellationToken);
        return Ok(data);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var site = await _dbContext.TouristSites.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        return site is null ? NotFound() : Ok(site);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] TouristSite input, CancellationToken cancellationToken)
    {
        _dbContext.TouristSites.Add(input);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = input.Id }, input);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] TouristSite input, CancellationToken cancellationToken)
    {
        var site = await _dbContext.TouristSites.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (site is null)
        {
            return NotFound();
        }

        site.Name = input.Name;
        site.City = input.City;
        site.SiteType = input.SiteType;
        site.Description = input.Description;
        site.MaxCapacity = input.MaxCapacity;
        site.IsActive = input.IsActive;

        await _dbContext.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var site = await _dbContext.TouristSites.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (site is null)
        {
            return NotFound();
        }

        _dbContext.TouristSites.Remove(site);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return NoContent();
    }
}

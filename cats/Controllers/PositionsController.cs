using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using cats.Data;
using cats.Models;
using cats.Services;
using Microsoft.EntityFrameworkCore;

namespace cats.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PositionsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogService _logService;

    public PositionsController(ApplicationDbContext context, ILogService logService)
    {
        _context = context;
        _logService = logService;
    }

    // GET: api/positions
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<Position>>> GetPositions()
    {
        await LogIfAuthenticated("GetPositions", Request.Path, "Retrieved list of positions");
        var positions = await _context.Positions.Include(p => p.Cat).ToListAsync();
        return positions;
    }

    // GET: api/positions/5
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<Position>> GetPosition(int id)
    {
        await LogIfAuthenticated("GetPosition", Request.Path, $"Requested position ID: {id}");
        var position = await _context.Positions.Include(p => p.Cat).FirstOrDefaultAsync(p => p.Id == id);

        if (position == null)
        {
            return NotFound();
        }

        return position;
    }

    // POST: api/positions
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<Position>> CreatePosition(Position position)
    {
        var cat = await _context.Cats.FindAsync(position.CatId);
        if (cat == null)
        {
            return NotFound("Invalid catId");
        }

        position.Cat = cat;

        _context.Positions.Add(position);
        await _context.SaveChangesAsync();

        await LogIfAuthenticated("CreatePosition", Request.Path, $"Created position ID: {position.Id}");

        return CreatedAtAction(nameof(GetPosition), new { id = position.Id }, position);
    }

    // PUT: api/positions/5
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdatePosition(int id, Position position)
    {
        if (id != position.Id)
        {
            return BadRequest();
        }

        var existingPosition = await _context.Positions.FindAsync(id);
        if (existingPosition == null)
        {
            return NotFound("Invalid positionId");
        }

        var cat = await _context.Cats.FindAsync(position.CatId);
        if (cat == null)
        {
            return BadRequest("Invalid CatId");
        }
        
        existingPosition.Price = position.Price;
        existingPosition.CatId = position.CatId;

        _context.Entry(existingPosition).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
            await LogIfAuthenticated("UpdatePosition", Request.Path, $"Updated position ID: {position.Id}");
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!PositionExists(id))
            {
                return NotFound("Invalid positionId");
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    // DELETE: api/positions/5
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeletePosition(int id)
    {
        var position = await _context.Positions.FindAsync(id);
        if (position == null)
        {
            return NotFound();
        }

        _context.Positions.Remove(position);
        await _context.SaveChangesAsync();

        await LogIfAuthenticated("DeletePosition", Request.Path, $"Deleted position ID: {id}");

        return NoContent();
    }

    // POST: api/positions/purchase/5
    [HttpPost("purchase/{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> PurchasePosition(int id)
    {
        var position = await _context.Positions.Include(p => p.Cat).FirstOrDefaultAsync(p => p.Id == id);
        if (position == null)
        {
            return NotFound();
        }
        
        _context.Positions.Remove(position);
        if (position.Cat != null)
        {
            _context.Cats.Remove(position.Cat);
        }

        await _context.SaveChangesAsync();
        await LogIfAuthenticated("PurchasePosition", Request.Path, $"Purchased position ID: {id}");

        return Ok("Position and associated cat purchased successfully.");
    }

    private async Task LogIfAuthenticated(string operation, string requestUrl, string description)
    {
        var userName = User.Identity.IsAuthenticated ? User.Identity.Name : "[anonymous]";
        await _logService.LogAsync(userName, requestUrl, description);
    }

    private bool PositionExists(int id)
    {
        return _context.Positions.Any(e => e.Id == id);
    }
}

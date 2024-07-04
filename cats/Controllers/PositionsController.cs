using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using cats.Data;
using cats.Models;
using Microsoft.EntityFrameworkCore;

namespace cats.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PositionsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public PositionsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/positions
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<Position>>> GetPositions()
    {
        return await _context.Positions.Include(p => p.Cat).ToListAsync();
    }

    // GET: api/positions/5
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<Position>> GetPosition(int id)
    {
        var position = await _context.Positions.Include(p => p.Cat).FirstOrDefaultAsync(p => p.Id == id);

        if (position == null)
        {
            return NotFound("No position with such id.");
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

        return CreatedAtAction(nameof(GetPosition), new { id = position.Id }, position);
    }

    // PUT: api/positions/5
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdatePosition(int id, Position position)
    {
        if (id != position.Id)
        {
            return NotFound("No position with such id.");
        }
        
        var cat = await _context.Cats.FindAsync(position.CatId);
        if (cat == null)
        {
            return BadRequest("Invalid CatId");
        }

        _context.Entry(position).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
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
    
    // POST: api/positions/purchase/5
    [HttpPost("purchase/{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> PurchasePosition(int id)
    {
        var position = await _context.Positions.Include(p => p.Cat).FirstOrDefaultAsync(p => p.Id == id);
        if (position == null)
        {
            return NotFound("No position with such id.");
        }
        
        _context.Positions.Remove(position);
        if (position.Cat != null)
        {
            _context.Cats.Remove(position.Cat);
        }

        await _context.SaveChangesAsync();

        return Ok("Position and associated cat purchased successfully.");
    }

    // DELETE: api/positions/5
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeletePosition(int id)
    {
        var position = await _context.Positions.FindAsync(id);
        if (position == null)
        {
            return NotFound("No position with such id.");
        }

        _context.Positions.Remove(position);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool PositionExists(int id)
    {
        return _context.Positions.Any(e => e.Id == id);
    }
}
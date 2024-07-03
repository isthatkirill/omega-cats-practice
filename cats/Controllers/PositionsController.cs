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
            return NotFound();
        }

        return position;
    }

    // POST: api/positions
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<Position>> CreatePosition(Position position)
    {
        _context.Positions.Add(position);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetPosition", new { id = position.Id }, position);
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

        _context.Entry(position).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!PositionExists(id))
            {
                return NotFound();
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

        return NoContent();
    }

    private bool PositionExists(int id)
    {
        return _context.Positions.Any(e => e.Id == id);
    }
}
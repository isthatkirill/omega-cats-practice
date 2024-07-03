using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using cats.Models;

namespace cats.Controllers;

[ApiController]
    [Route("api/[controller]")]
    public class PositionsController : ControllerBase
    {
        [HttpGet("{id}")]
        public ActionResult<Position> GetPosition(int id)
        {
            var position = DataStore.Positions.FirstOrDefault(p => p.Id == id);
            if (position == null)
            {
                return NotFound();
            }
            return Ok(position);
        }

        [HttpPost("{id}/buy")]
        public ActionResult BuyPosition(int id)
        {
            var position = DataStore.Positions.FirstOrDefault(p => p.Id == id);
            if (position == null)
            {
                return NotFound();
            }
            DataStore.Positions.Remove(position);
            return Ok();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult CreatePosition([FromBody] Position position)
        {
            position.Id = DataStore.Positions.Count + 1;
            position.DateAdded = DateTime.UtcNow;
            position.DateModified = DateTime.UtcNow;
            DataStore.Positions.Add(position);
            return CreatedAtAction(nameof(GetPosition), new { id = position.Id }, position);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public ActionResult EditPosition(int id, [FromBody] Position updatedPosition)
        {
            var position = DataStore.Positions.FirstOrDefault(p => p.Id == id);
            if (position == null)
            {
                return NotFound();
            }
            position.Price = updatedPosition.Price;
            position.CatId = updatedPosition.CatId;
            position.DateModified = DateTime.UtcNow;
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public ActionResult DeletePosition(int id)
        {
            var position = DataStore.Positions.FirstOrDefault(p => p.Id == id);
            if (position == null)
            {
                return NotFound();
            }
            DataStore.Positions.Remove(position);
            return NoContent();
        }
    }
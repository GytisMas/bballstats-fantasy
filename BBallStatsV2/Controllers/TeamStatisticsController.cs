using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BBallStats.Data;
using BBallStats.Data.Entities;

namespace BBallStatsV2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeamStatisticsController : ControllerBase
    {
        private readonly ForumDbContext _context;

        public TeamStatisticsController(ForumDbContext context)
        {
            _context = context;
        }

        // GET: api/TeamStatistics
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TeamStatistic>>> GetTeamStatistics()
        {
            return await _context.TeamStatistics.ToListAsync();
        }

        // GET: api/TeamStatistics/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TeamStatistic>> GetTeamStatistic(int id)
        {
            var teamStatistic = await _context.TeamStatistics.FindAsync(id);

            if (teamStatistic == null)
            {
                return NotFound();
            }

            return teamStatistic;
        }

        // PUT: api/TeamStatistics/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTeamStatistic(int id, TeamStatistic teamStatistic)
        {
            if (id != teamStatistic.Id)
            {
                return BadRequest();
            }

            _context.Entry(teamStatistic).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TeamStatisticExists(id))
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

        // POST: api/TeamStatistics
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<TeamStatistic>> PostTeamStatistic(TeamStatistic teamStatistic)
        {
            _context.TeamStatistics.Add(teamStatistic);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTeamStatistic", new { id = teamStatistic.Id }, teamStatistic);
        }

        // DELETE: api/TeamStatistics/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTeamStatistic(int id)
        {
            var teamStatistic = await _context.TeamStatistics.FindAsync(id);
            if (teamStatistic == null)
            {
                return NotFound();
            }

            _context.TeamStatistics.Remove(teamStatistic);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TeamStatisticExists(int id)
        {
            return _context.TeamStatistics.Any(e => e.Id == id);
        }
    }
}

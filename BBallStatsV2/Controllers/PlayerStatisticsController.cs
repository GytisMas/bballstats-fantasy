using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BBallStats.Data;
using BBallStats.Data.Entities;
using static BBallStats.Data.Entities.PlayerStatistic;

namespace BBallStatsV2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayerStatisticsController : ControllerBase
    {
        private readonly ForumDbContext _context;

        public PlayerStatisticsController(ForumDbContext context)
        {
            _context = context;
        }

        // GET: api/PlayerStatistics
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PlayerStatistic>>> GetPlayerStatistics()
        {
            return await _context.PlayerStatistics
                .ToListAsync();
        }

        [HttpGet("~/api/Teams/{teamId}/Players/{playerId}/[controller]")]
        public async Task<ActionResult<IEnumerable<PlayerStatisticDto>>> GetPlayers(string teamId, string playerId)
        {
            return await _context.PlayerStatistics
                .Where(p => p.PlayerId.Equals(playerId))
                .Select(s => new PlayerStatisticDto(s.Id, s.Value, s.StatisticId, s.PlayerId))
                .ToListAsync();
        }

        // GET: api/PlayerStatistics/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PlayerStatistic>> GetPlayerStatistic(int id)
        {
            var playerStatistic = await _context.PlayerStatistics.FindAsync(id);

            if (playerStatistic == null)
            {
                return NotFound();
            }

            return playerStatistic;
        }

        // PUT: api/PlayerStatistics/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPlayerStatistic(int id, PlayerStatistic playerStatistic)
        {
            if (id != playerStatistic.Id)
            {
                return BadRequest();
            }

            _context.Entry(playerStatistic).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PlayerStatisticExists(id))
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

        // POST: api/PlayerStatistics
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<PlayerStatistic>> PostPlayerStatistic(PlayerStatistic playerStatistic)
        {
            _context.PlayerStatistics.Add(playerStatistic);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPlayerStatistic", new { id = playerStatistic.Id }, playerStatistic);
        }

        // DELETE: api/PlayerStatistics/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlayerStatistic(int id)
        {
            var playerStatistic = await _context.PlayerStatistics.FindAsync(id);
            if (playerStatistic == null)
            {
                return NotFound();
            }

            _context.PlayerStatistics.Remove(playerStatistic);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PlayerStatisticExists(int id)
        {
            return _context.PlayerStatistics.Any(e => e.Id == id);
        }
    }
}

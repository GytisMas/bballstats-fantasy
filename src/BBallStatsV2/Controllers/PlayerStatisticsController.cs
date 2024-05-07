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
using Microsoft.AspNetCore.Authorization;

namespace BBallStatsV2.Controllers
{
    [Route("api/teams/{teamId}/players/{playerId}/[controller]")]
    [ApiController]
    public class PlayerStatisticsController : ControllerBase
    {
        private readonly ForumDbContext _context;

        public PlayerStatisticsController(ForumDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PlayerStatisticDto>>> GetPlayersStatistics(string teamId, string playerId)
        {
            return await _context.PlayerStatistics
                .Where(p => p.PlayerId.Equals(playerId))
                .Select(s => new PlayerStatisticDto(s.Id, s.Value, s.AttemptValue, s.GameCount, s.StatisticId, s.PlayerId))
                .ToListAsync();
        }

        // GET: api/PlayerStatistics/5
        [HttpGet("{statId}")]
        public async Task<ActionResult<PlayerStatisticDto>> GetPlayerStatistic(int statId)
        {
            var playerStatistic = await _context.PlayerStatistics.FindAsync(statId);

            if (playerStatistic == null)
            {
                return NotFound();
            }

            return Ok(new PlayerStatisticDto(playerStatistic.Id, playerStatistic.Value, playerStatistic.AttemptValue, 
                playerStatistic.GameCount, playerStatistic.StatisticId, playerStatistic.PlayerId));
        }

        // PUT: api/PlayerStatistics/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPut("{statId}")]
        public async Task<IActionResult> PutPlayerStatistic(int statId, UpdatePlayerStatisticDto dto)
        {
            var playerStatistic = await _context.PlayerStatistics.FindAsync(statId);
            if (playerStatistic == null)
            {
                return NotFound();
            }

            playerStatistic.AttemptValue = dto.AttemptValue;
            playerStatistic.Value = dto.Value;
            playerStatistic.GameCount = dto.GameCount;
            playerStatistic.StatisticId = dto.StatType;

            _context.Update(playerStatistic);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/PlayerStatistics
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<PlayerStatistic>> PostPlayerStatistic(PlayerStatistic playerStatistic)
        {
            _context.PlayerStatistics.Add(playerStatistic);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPlayerStatistic", new { id = playerStatistic.Id }, playerStatistic);
        }

        // DELETE: api/PlayerStatistics/5
        [Authorize]
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

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
    public class TeamsController : ControllerBase
    {
        private readonly ForumDbContext _context;

        public TeamsController(ForumDbContext context)
        {
            _context = context;
        }

        // GET: api/Teams
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Team>>> GetTeams()
        {
            return await _context.Teams.ToListAsync();
        }

        // GET: api/Teams
        [HttpGet("ByDates")]
        public async Task<ActionResult<List<TeamDto>>> GetTeamsByUpcomingGames(DateTime startDate, DateTime endDate)
        {
            var matches = await _context.Matches
                .Where(m => m.MatchDate >= startDate && m.MatchDate <= endDate)
                .Select(m => new { AwayTeamId = m.AwayTeamId, HomeTeamId = m.HomeTeamId })
                .ToListAsync();
            if (matches == null || !matches.Any()) 
            { 
                return NotFound();
            }

            var teamIds = matches.Select(m => m.HomeTeamId).ToList();
            teamIds.AddRange(matches.Select(m => m.AwayTeamId).ToList());
            teamIds = teamIds.Distinct().ToList();

            return await _context.Teams.Where(t => teamIds.Contains(t.Id))
                .Select(t => new TeamDto(t.Id, t.Name)).ToListAsync();
        }

        // GET: api/Teams/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Team>> GetTeam(string id)
        {
            var team = await _context.Teams.FindAsync(id);

            if (team == null)
            {
                return NotFound();
            }

            return team;
        }

        // PUT: api/Teams/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTeam(string id, Team team)
        {
            if (id != team.Id)
            {
                return BadRequest();
            }

            _context.Entry(team).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TeamExists(id))
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

        // POST: api/Teams
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Team>> PostTeam(Team team)
        {
            _context.Teams.Add(team);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (TeamExists(team.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetTeam", new { id = team.Id }, team);
        }

        // DELETE: api/Teams/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTeam(string id)
        {
            var team = await _context.Teams.FindAsync(id);
            if (team == null)
            {
                return NotFound();
            }

            _context.Teams.Remove(team);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TeamExists(string id)
        {
            return _context.Teams.Any(e => e.Id == id);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BBallStats.Data;
using BBallStats.Data.Entities;
using static BBallStats.Shared.Utils.DTOs;
using Microsoft.AspNetCore.Authorization;

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
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Team>>> GetTeams()
        {
            return await _context.Teams.ToListAsync();
        }

        // GET: api/Teams
        [Authorize]
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
        [Authorize]
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
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTeam(string id, TeamNameDto dto)
        {
            var team = await _context.Teams.FindAsync(id);
            if (team == null)
                return NotFound();

            team.Name = dto.Name;

            _context.Update(team);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // POST: api/Teams
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Team>> CreateTeams(TeamDto[] newTeams)
        {
            var teams = await _context.Teams.ToListAsync();

            foreach (var newTeam in newTeams)
            {
                var existingTeam = teams.FirstOrDefault(t => t.Id == newTeam.Id);
                if (existingTeam == null)
                    _context.Teams.Add(
                        new Team()
                        {
                            Id = newTeam.Id,
                            Name = newTeam.Name,
                        }
                    );
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/Teams/5
        [Authorize]
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
    }
}

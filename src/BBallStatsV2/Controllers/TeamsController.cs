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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TeamDto>>> GetTeams()
        {
            return await _context.Teams.Select(t => new TeamDto(t.Id, t.Name)).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TeamDto>> GetTeam(string id)
        {
            var team = await _context.Teams.FindAsync(id);

            if (team == null)
            {
                return NotFound();
            }

            return Ok(new TeamDto(team.Id, team.Name));
        }

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
            return Ok(new TeamDto(team.Id, team.Name));
        }

        [HttpPut]
        public async Task<ActionResult<TeamWithPlayersDto[]>> CreateTeamsAndPlayers(TeamWithPlayersDto[] newTeams)
        {
            var VisitedPlayers = new List<string>();
            foreach (var newOrExistingTeam in newTeams)
            {
                var existingTeam = await _context.Teams.FindAsync(newOrExistingTeam.Id);
                if (existingTeam == null)
                {
                    _context.Teams.Add(
                        new Team()
                        {
                            Id = newOrExistingTeam.Id,
                            Name = newOrExistingTeam.Name,
                        }
                    );
                }
                else if (existingTeam.Name !=  newOrExistingTeam.Name)
                {
                    existingTeam.Name = newOrExistingTeam.Name;
                    _context.Teams.Update(existingTeam);
                }

                foreach (var player in newOrExistingTeam.Players)
                {
                    if (VisitedPlayers.Contains(player.Id))
                    {
                        continue;
                    }
                    VisitedPlayers.Add(player.Id);
                    if (player.Id == "011941")
                    {
                        Console.WriteLine("+");
                    }

                    var playerRole =
                        player.Role == "Guard" ? PlayerRole.Guard :
                        player.Role == "Forward" ? PlayerRole.Forward :
                        player.Role == "Center" ? PlayerRole.Center :
                        PlayerRole.Other;

                    var existingPlayer = await _context.Players.FindAsync(player.Id);
                    if (existingPlayer != null)
                    {
                        existingPlayer.Role = playerRole;
                        existingPlayer.Name = player.Name;
                        _context.Players.Update(existingPlayer);
                        continue;
                    }

                    var newPlayer = new Player()
                    {
                        Id = player.Id,
                        Name = player.Name,
                        Role = playerRole,
                        SkippedLastGame = false,
                        CurrentTeamId = newOrExistingTeam.Id 
                    };
                    _context.Players.Add(newPlayer);
                }
            }

            await _context.SaveChangesAsync();
            return Ok(newTeams);
        }

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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BBallStats.Data;
using BBallStats.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using static BBallStats.Shared.Utils.DTOs;

namespace BBallStatsV2.Controllers
{
    [Route("api/teams/{teamId}/[controller]")]
    [ApiController]
    public class PlayersController : ControllerBase
    {
        private readonly ForumDbContext _context;

        public PlayersController(ForumDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PlayerShortDto>>> GetPlayers(string teamId)
        {
            return await _context.Players
                .Where(p => p.CurrentTeamId.Equals(teamId))
                .Select(p => new PlayerShortDto(p.Id, p.Name, p.Role))
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PlayerDto>> GetPlayer(string id)
        {
            var player = await _context.Players.FindAsync(id);

            if (player == null)
            {
                return NotFound();
            }

            return Ok(new PlayerDto(player.Id, player.Name, player.Role, player.CurrentTeamId, player.ForbidAutoUpdate));
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPlayer(string id, UpdatePlayerDto dto)
        {
            var player = await _context.Players.FindAsync(id);
            if (player == null)
                return NotFound();

            player.Name = dto.Name;
            player.Role = dto.Role;
            player.CurrentTeamId = dto.TeamId;
            player.ForbidAutoUpdate = dto.ForbidAutoUpdate;

            _context.Players.Update(player);
            await _context.SaveChangesAsync();

            return Ok(new PlayerDto(player.Id, player.Name, player.Role, player.CurrentTeamId, player.ForbidAutoUpdate));
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Player>> PostPlayer(string teamId, CreatePlayerDto dto)
        {
            var player = new Player() { Name = dto.Name };
            player.Id = dto.Id;
            player.Name = dto.Name;
            player.Role = dto.Role;
            player.CurrentTeamId = teamId;
            player.ForbidAutoUpdate = dto.ForbidAutoUpdate;

            _context.Players.Add(player);
            await _context.SaveChangesAsync();

            return Created($"/api/players/{player.Id}", new PlayerDto(player.Id, player.Name, player.Role, player.CurrentTeamId, player.ForbidAutoUpdate));
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlayer(string id)
        {
            var player = await _context.Players.FindAsync(id);
            if (player == null)
            {
                return NotFound();
            }

            _context.Players.Remove(player);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PlayerExists(string id)
        {
            return _context.Players.Any(e => e.Id == id);
        }
    }
}

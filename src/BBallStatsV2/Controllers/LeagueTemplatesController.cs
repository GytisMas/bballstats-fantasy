using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BBallStats.Data;
using BBallStatsV2.Data.Entities;
using BBallStatsV2.DTOs;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace BBallStatsV2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeagueTemplatesController : ControllerBase
    {
        private readonly ForumDbContext _context;

        public LeagueTemplatesController(ForumDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<LeagueTemplateNameIdDto>>> GetLeagueTemplates()
        {
            return await _context.LeagueTemplates.Select(t => new LeagueTemplateNameIdDto(t.Id, t.Name)).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<LeagueTemplateDto>> GetLeagueTemplate(int id)
        {
            var leagueTemplate = await _context.LeagueTemplates
                .Include(t => t.Roles)
                .ThenInclude(r => r.Statistics)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (leagueTemplate == null)
            {
                return NotFound();
            }

            return new LeagueTemplateDto(leagueTemplate.Id, leagueTemplate.Name, leagueTemplate.BenchMultiplier, leagueTemplate.TeamWinPoints, leagueTemplate.TeamLosePoints,
                leagueTemplate.Roles.Select(r => new LeagueRoleWithIdDto(r.Id, r.Name, r.RoleToReplaceId,
                    r.Statistics.Select(s => new LeagueStatDto(s.PointsPerStat, s.StatisticId)).ToArray()
                )).ToArray());
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<LeagueTemplate>> PostLeagueTemplate(LeagueTemplateUpsertDto leagueTemplateDto)
        {
            var userId = HttpContext.User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            var user = await _context.Users
                .FindAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var userBalance = await _context.Transactions
                .Where(t => t.SenderId == userId || t.RecipientId == userId)
                .SumAsync(t => t.RecipientId == userId ? t.Amount : -t.Amount);

            var templateCreationPrice = 500;

            if (userBalance < templateCreationPrice)
            {
                return UnprocessableEntity("InsufficientFunds");
            }

            var existingNameLeague = await _context.LeagueTemplates.FirstOrDefaultAsync(x => x.Name.Equals(leagueTemplateDto.Name));
            if (existingNameLeague != null)
                return BadRequest("League template with name already exists");

            List<int> AllStatIds = new List<int>();
            foreach (LeagueRoleWithIdDto leagueRoleDto in leagueTemplateDto.LeagueRoles)
                foreach (LeagueStatDto stat in leagueRoleDto.LeagueStats)
                    AllStatIds.Add(stat.StatisticId);
            AllStatIds = AllStatIds.Distinct().ToList();
            var Statistics = await _context.Statistics.Where(x => AllStatIds.Contains(x.Id)).ToListAsync();
            if (Statistics == null || Statistics.Count != AllStatIds.Count)
            {
                return BadRequest("Invalid statistics used");
            }

            var leagueTemplate = new LeagueTemplate()
            {
                Name = leagueTemplateDto.Name,
                BenchMultiplier = leagueTemplateDto.BenchMultiplier / (double)100,
                TeamWinPoints = leagueTemplateDto.TeamWinPoints,
                TeamLosePoints = leagueTemplateDto.TeamLosePoints,
                Roles = leagueTemplateDto.LeagueRoles.Select(x => new LeaguePlayerRole()
                {
                    Name = x.Name,
                }).ToList()
            };
            for (int i = 0; i < leagueTemplate.Roles.Count; i++)
            {
                var role = leagueTemplate.Roles[i];
                var roleDto = leagueTemplateDto.LeagueRoles[i];
                role.RoleToReplace = roleDto.RoleToReplaceIndex != null ? leagueTemplate.Roles[(int)roleDto.RoleToReplaceIndex] : null;
                role.Statistics = roleDto.LeagueStats.Select(x => new LeagueStatisticToCount()
                {
                    PointsPerStat = x.PointsPerStat,
                    LeaguePlayerRole = role,
                    StatisticId = x.StatisticId
                }).ToList();
            }

            var creationTransaction = new Transaction
            {
                TransactionType = TransactionType.LeagueTemplateCreation,
                Amount = templateCreationPrice,
                Sender = user,
                RecipientId = null,
                Date = DateTime.UtcNow
            };

            _context.Transactions.Add(creationTransaction);
            _context.LeagueTemplates.Add(leagueTemplate);
            await _context.SaveChangesAsync();

            return Ok(new LeagueTemplateDto(leagueTemplate.Id, leagueTemplate.Name, leagueTemplate.BenchMultiplier, leagueTemplate.TeamWinPoints, leagueTemplate.TeamLosePoints,
                leagueTemplate.Roles.Select(role => new LeagueRoleWithIdDto(role.Id, role.Name, role.RoleToReplaceId,
                    role.Statistics.Select(stat => new LeagueStatDto(stat.PointsPerStat, stat.StatisticId)).ToArray()
                )).ToArray()
            ));
        }

        // DELETE: api/LeagueTemplates/5
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLeagueTemplate(int id)
        {
            var leagueTemplate = await _context.LeagueTemplates.FindAsync(id);
            if (leagueTemplate == null)
            {
                return NotFound();
            }

            _context.LeagueTemplates.Remove(leagueTemplate);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool LeagueTemplateExists(int id)
        {
            return _context.LeagueTemplates.Any(e => e.Id == id);
        }
    }
}

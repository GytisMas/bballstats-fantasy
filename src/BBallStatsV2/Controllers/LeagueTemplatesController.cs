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

        // GET: api/LeagueTemplates
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LeagueTemplate>>> GetLeagueTemplates()
        {
            return await _context.LeagueTemplates.ToListAsync();
        }

        // GET: api/LeagueTemplates/5
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

            return new LeagueTemplateDto(leagueTemplate.Name, leagueTemplate.BenchMultiplier, leagueTemplate.TeamWinPoints, leagueTemplate.TeamLosePoints,
                leagueTemplate.Roles.Select(r => new LeagueRoleWithIdDto(r.Id, r.Name, r.RoleToReplaceId,
                    r.Statistics.Select(s => new LeagueStatDto(s.PointsPerStat, s.StatisticId)).ToArray()
                )).ToArray());
        }

        // PUT: api/LeagueTemplates/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLeagueTemplate(int id, LeagueTemplate leagueTemplate)
        {
            if (id != leagueTemplate.Id)
            {
                return BadRequest();
            }

            _context.Entry(leagueTemplate).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LeagueTemplateExists(id))
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

        // POST: api/LeagueTemplates
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<LeagueTemplate>> PostLeagueTemplate(LeagueTemplateDto leagueTemplateDto)
        {
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

            _context.LeagueTemplates.Add(leagueTemplate);
            await _context.SaveChangesAsync();

            return Ok(new LeagueTemplateDto(leagueTemplate.Name, leagueTemplate.BenchMultiplier, leagueTemplate.TeamWinPoints, leagueTemplate.TeamLosePoints,
                leagueTemplate.Roles.Select(role => new LeagueRoleWithIdDto(role.Id, role.Name, role.RoleToReplaceId,
                    role.Statistics.Select(stat => new LeagueStatDto(stat.PointsPerStat, stat.StatisticId)).ToArray()
                )).ToArray()
            ));
        }

        // DELETE: api/LeagueTemplates/5
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BBallStats.Data;
using BBallStats.Data.Entities;
using static BBallStats.Data.Entities.AlgorithmImpression;
using System.Threading;
using BBallStats2.Auth.Model;
using System.Net.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace BBallStatsV2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AlgorithmImpressionsController : ControllerBase
    {
        private readonly ForumDbContext _context;

        public AlgorithmImpressionsController(ForumDbContext context)
        {
            _context = context;
        }

        // GET: api/AlgorithmImpressions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AlgorithmImpressionDto>>> GetAlgorithmImpressions(int customStatisticId)
        {
            var customStatistic = await _context.CustomStatistics.FirstOrDefaultAsync(r => r.Id == customStatisticId);
            if (customStatistic == null)
                return NotFound();


            var algorithmImpressions = (await _context.AlgorithmImpressions.ToListAsync())
                .Where(o => o.CustomStatistic == customStatistic)
                .Select(o => new AlgorithmImpressionDto(
                    o.Id,
                    o.Positive,
                    customStatisticId,
                    o.UserId));

            return Ok(algorithmImpressions);
        }

        // GET: api/AlgorithmImpressions/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AlgorithmImpressionDto>> GetAlgorithmImpression(int impressionId)
        {
            var algorithmImpression = await _context.AlgorithmImpressions.FindAsync(impressionId);

            if (algorithmImpression == null)
            {
                return NotFound();
            }

            return Ok(new AlgorithmImpressionDto(algorithmImpression.Id, algorithmImpression.Positive, algorithmImpression.CustomStatisticId, algorithmImpression.UserId));
        }

        // PUT: api/AlgorithmImpressions/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAlgorithmImpression(string userId, int impressionId, UpdateAlgorithmImpressionDto updateImpressionDto)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return NotFound();

            var algorithmImpression = await _context.AlgorithmImpressions.FirstOrDefaultAsync(r => r.Id == impressionId);
            if (algorithmImpression == null)
                return NotFound();

            //var impressionUser = await _context.Users.FirstOrDefaultAsync(r => r.Id == HttpContext.User.FindFirstValue(JwtRegisteredClaimNames.Sub));
            //if (impressionUser == null)
            //    return NotFound();

            if (!HttpContext.User.IsInRole(ForumRoles.Regular)
                /*|| impressionUser.Id != algorithmImpression.UserId*/)
                return Forbid();

            algorithmImpression.Positive = updateImpressionDto.Positive;

            _context.Update(algorithmImpression);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/AlgorithmImpressions
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        // TODO: update with dto
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<AlgorithmImpression>> PostAlgorithmImpression(AlgorithmImpression algorithmImpression)
        {
            _context.AlgorithmImpressions.Add(algorithmImpression);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAlgorithmImpression", new { id = algorithmImpression.Id }, algorithmImpression);
        }

        // DELETE: api/AlgorithmImpressions/5
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAlgorithmImpression(int id)
        {
            var algorithmImpression = await _context.AlgorithmImpressions.FindAsync(id);
            if (algorithmImpression == null)
            {
                return NotFound();
            }

            _context.AlgorithmImpressions.Remove(algorithmImpression);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AlgorithmImpressionExists(int id)
        {
            return _context.AlgorithmImpressions.Any(e => e.Id == id);
        }
    }
}

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
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace BBallStatsV2.Controllers
{
    [Route("api/users/{userId}/customStatistics/{customStatisticId}/[controller]")]
    [ApiController]
    public class AlgorithmImpressionsController : ControllerBase
    {
        private readonly ForumDbContext _context;
        private readonly UserManager<ForumRestUser> _userManager;

        public AlgorithmImpressionsController(ForumDbContext context, UserManager<ForumRestUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AlgorithmImpressionDto>>> GetCustomStatisticImpressions(string userId, int customStatisticId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound();

            var ratingAlgorithm = await _context.CustomStatistics.FirstOrDefaultAsync(r => r.Id == customStatisticId && r.User.Id == userId);
            if (ratingAlgorithm == null)
                return NotFound();


            var algorithmImpressions = (await _context.AlgorithmImpressions.ToListAsync())
                .Where(o => o.CustomStatistic == ratingAlgorithm)
                .Select(o => new AlgorithmImpressionDto(
                    o.Id,
                    o.Positive,
                    customStatisticId,
                    o.UserId));

            return Ok(algorithmImpressions);
        }

        [HttpGet("{impressionId}")]
        public async Task<ActionResult<AlgorithmImpressionDto>> GetCustomStatisticImpression(string userId, int customStatisticId, int impressionId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound();

            var ratingAlgorithm = await _context.CustomStatistics.FirstOrDefaultAsync(r => r.Id == customStatisticId && r.User.Id == userId);
            if (ratingAlgorithm == null)
                return NotFound();

            var algorithmImpression = await _context.AlgorithmImpressions.FirstOrDefaultAsync(r => r.Id == impressionId && r.CustomStatistic.Id == customStatisticId);
            if (algorithmImpression == null)
                return NotFound();


            return Ok(new AlgorithmImpressionDto(algorithmImpression.Id, algorithmImpression.Positive, customStatisticId, algorithmImpression.UserId));
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<AlgorithmImpressionDto>> CreateCustomStatisticImpression(string userId, int customStatisticId, CreateAlgorithmImpressionDto dto)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound();

            var ratingAlgorithm = await _context.CustomStatistics.FirstOrDefaultAsync(r => r.Id == customStatisticId && r.User.Id == userId);
            if (ratingAlgorithm == null)
                return NotFound();

            var impressionUser = await _context.Users.FirstOrDefaultAsync(r => r.Id == HttpContext.User.FindFirstValue(JwtRegisteredClaimNames.Sub));
            if (impressionUser == null)
                return NotFound();

            if (!HttpContext.User.IsInRole(ForumRoles.Regular))
                return Forbid();

            var existingImpression = await _context.AlgorithmImpressions.FirstOrDefaultAsync(i => i.CustomStatistic.Id == ratingAlgorithm.Id && i.UserId == impressionUser.Id);
            if (existingImpression != null)
            {
                return UnprocessableEntity("User has already rated this algorithm");
            }

            var algorithmImpression = new AlgorithmImpression()
            {
                Positive = dto.Positive,
                CustomStatistic = ratingAlgorithm,
                UserId = impressionUser.Id
            };

            _context.AlgorithmImpressions.Add(algorithmImpression);
            await _context.SaveChangesAsync();

            return Created($"/api/Users/{user.Id}/CustomStatistics/{ratingAlgorithm.Id}/AlgorithmImpressions/{algorithmImpression.Id}",
                new AlgorithmImpressionDto(algorithmImpression.Id, algorithmImpression.Positive, customStatisticId, algorithmImpression.UserId));
        }

        [Authorize]
        [HttpPut("{impressionId}")]
        public async Task<ActionResult<AlgorithmImpressionDto>> UpdateCustomStatisticImpression(string userId, int customStatisticId, int impressionId, UpdateAlgorithmImpressionDto dto)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound();

            var ratingAlgorithm = await _context.CustomStatistics.FirstOrDefaultAsync(r => r.Id == customStatisticId && r.User.Id == userId);
            if (ratingAlgorithm == null)
                return NotFound();

            var algorithmImpression = await _context.AlgorithmImpressions.FirstOrDefaultAsync(r => r.Id == impressionId && r.CustomStatistic.Id == customStatisticId);
            if (algorithmImpression == null)
                return NotFound();

            var impressionUser = await _context.Users.FirstOrDefaultAsync(r => r.Id == HttpContext.User.FindFirstValue(JwtRegisteredClaimNames.Sub));
            if (impressionUser == null)
                return NotFound();

            if (!HttpContext.User.IsInRole(ForumRoles.Regular)
                || impressionUser.Id != algorithmImpression.UserId)
                return Forbid();

            algorithmImpression.Positive = dto.Positive;

            _context.Update(algorithmImpression);
            await _context.SaveChangesAsync();

            return Ok(new AlgorithmImpressionDto(algorithmImpression.Id, algorithmImpression.Positive, customStatisticId, algorithmImpression.UserId));
        }

        [Authorize]
        [HttpDelete("{impressionId}")]
        public async Task<ActionResult<AlgorithmImpressionDto>> DeleteCustomStatisticImpression(string userId, int customStatisticId, int impressionId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound();

            var ratingAlgorithm = await _context.CustomStatistics.FirstOrDefaultAsync(r => r.Id == customStatisticId && r.User.Id == userId);
            if (ratingAlgorithm == null)
                return NotFound();

            var algorithmImpression = await _context.AlgorithmImpressions.FirstOrDefaultAsync(r => r.Id == impressionId && r.CustomStatistic.Id == customStatisticId);
            if (algorithmImpression == null)
                return NotFound();

            if (!HttpContext.User.IsInRole(ForumRoles.Regular)
                || HttpContext.User.FindFirstValue(JwtRegisteredClaimNames.Sub) != algorithmImpression.UserId)
                return Forbid();

            _context.Remove(algorithmImpression);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}

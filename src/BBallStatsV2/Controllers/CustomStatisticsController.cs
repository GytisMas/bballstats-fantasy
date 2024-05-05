﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BBallStats.Data;
using BBallStats.Data.Entities;
using BBallStats2.Auth.Model;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using System.Threading;
using System.Net.Http;
using System.Collections.Immutable;
using BBallStatsV2.DTOs;
using BBallStatsV2.Data.Entities;

namespace BBallStatsV2.Controllers
{
    [Route("/api/users/{userId}/[controller]")]
    [ApiController]
    public class CustomStatisticsController : ControllerBase
    {
        private readonly ForumDbContext _context;
        private readonly UserManager<ForumRestUser> _userManager;

        public CustomStatisticsController(ForumDbContext context, UserManager<ForumRestUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet("~/api/customStatistics")]
        public async Task<ActionResult<PagedListDto<CustomStatisticDto>>> GetCustomStatistics(int pageIndex = 1, int pageSize = 15)
        {
            var customStatistics = await _context.CustomStatistics
                .OrderByDescending(s => s.Id)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Select(o => new CustomStatisticDto(o.Id, o.Name, o.Formula, o.Status, o.UserId))
                .ToListAsync();

            var totalCount = await _context.CustomStatistics.CountAsync();
            int pageCount = totalCount / pageSize;
            if (totalCount % pageSize != 0)
                pageCount++;

            return Ok(new PagedListDto<CustomStatisticDto>(customStatistics, pageIndex, pageCount));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CustomStatisticDto>>> GetUserCustomStatistics(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound();

            var customStatistics = await _context.CustomStatistics
                .Where(o => o.UserId == userId)
                .Select(o => new CustomStatisticDto(o.Id, o.Name, o.Formula, o.Status, userId))
                .ToListAsync();

            return Ok(customStatistics);
        }

        [HttpGet("{customStatisticId}")]
        public async Task<ActionResult<CustomStatisticDto>> GetCustomStatistic(string userId, int customStatisticId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound();

            var customStatistic = await _context.CustomStatistics
                .Include(s => s.Statistics.OrderBy(o => o.Id))
                .SingleOrDefaultAsync(s => s.Id == customStatisticId && s.UserId == userId);

            var customStatIndexes = await _context.CustomStatisticRegularStatistic
                .Where(s => s.CustomStatisticId == customStatisticId)
                .OrderBy(s => s.Id)
                .Select(s => s.RegularStatisticId)
                .ToListAsync();

            customStatistic.Statistics = customStatistic.Statistics.OrderBy(s => customStatIndexes.IndexOf(s.Id)).ToList();

            if (customStatistic == null)
            {
                return NotFound();
            }

            return Ok(new CustomStatisticWithStatsDto(customStatistic.Id, customStatistic.Name, 
                customStatistic.Formula, customStatistic.Status, customStatistic.UserId,
                customStatistic.Statistics.Select(s => s.Id).ToArray()));
        }

        [HttpPut("{customStatisticId}")]
        public async Task<IActionResult> UpdateCustomStatistic(int customStatisticId, string userId, UpdateCustomStatisticDto dto)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound();

            var customStatistic = await _context.CustomStatistics
                .Include(s => s.Statistics)
                .FirstOrDefaultAsync(r => r.Id == customStatisticId && r.User.Id == userId);
            if (customStatistic == null)
                return NotFound();

            if (!HttpContext.User.IsInRole(ForumRoles.Regular)
                || HttpContext.User.FindFirstValue(JwtRegisteredClaimNames.Sub) != userId)
                return Forbid();


            List<int> newFormulaStatIds = dto.Formula.Split(") ")[0]
                .Remove(0, 1)
                .Split(" ", StringSplitOptions.RemoveEmptyEntries)
                .Select(int.Parse).ToList();
            var NewStats = await _context.RegularStatistics
                .Where(s => newFormulaStatIds.Contains(s.Id))
                .ToListAsync();

            NewStats.Sort((a, b)
                => newFormulaStatIds.IndexOf(a.Id)
                    .CompareTo(newFormulaStatIds.IndexOf(b.Id)));

            if (NewStats.Count < newFormulaStatIds.Count)
                return NotFound();

            //var customStatisticIndexes = _context.CustomStatisticRegularStatistic
            //    .Where(s => s.CustomStatisticId == customStatisticId);
            customStatistic.Statistics = new List<RegularStatistic>();
            _context.CustomStatistics.Update(customStatistic);
            await _context.SaveChangesAsync();

            //_context.CustomStatisticRegularStatistic.RemoveRange(customStatisticIndexes);

            customStatistic.Statistics = NewStats;
            customStatistic.Name = dto.Name;
            customStatistic.Formula = dto.Formula;
            customStatistic.Status = dto.Status;

            _context.CustomStatistics.Update(customStatistic);
            await _context.SaveChangesAsync();

            return Ok(new CustomStatisticDto(customStatistic.Id, customStatistic.Name, customStatistic.Formula, customStatistic.Status, customStatistic.UserId));
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<CustomStatisticDto>> CreateCustomStatistic(string userId, CreateCustomStatisticDto customStatisticDto)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound("user not found");
            if (!HttpContext.User.IsInRole(ForumRoles.Regular)
                || HttpContext.User.FindFirstValue(JwtRegisteredClaimNames.Sub) != userId)
            {
                return UnprocessableEntity($"{HttpContext.User.FindFirstValue(JwtRegisteredClaimNames.Sub)} | {userId}");
            }

            List<int> formulaStatIds = customStatisticDto.Formula.Split(") ")[0]
                .Remove(0, 1)
                .Split(" ", StringSplitOptions.RemoveEmptyEntries)
                .Select(Int32.Parse).ToList<int>();

            var Stats = await _context.RegularStatistics
                .Where(s => formulaStatIds.Contains(s.Id))
                .ToListAsync();

            Stats.Sort((a, b) 
                => formulaStatIds.IndexOf(a.Id)
                    .CompareTo(formulaStatIds.IndexOf(b.Id)));

            if (Stats.Count < formulaStatIds.Count)
                return NotFound("statistic not found");

            var customStatistic = new CustomStatistic()
            {
                Name = customStatisticDto.Name,
                Formula = customStatisticDto.Formula,
                Status = customStatisticDto.Status,
                UserId = HttpContext.User.FindFirstValue(JwtRegisteredClaimNames.Sub)
            };

            customStatistic.Statistics = Stats;

            _context.CustomStatistics.Add(customStatistic);

            await _context.SaveChangesAsync();

            return Created($"/api/Users/{user.Id}/CustomStatistics/{customStatistic.Id}",
                new CustomStatisticDto(customStatistic.Id, customStatistic.Name, customStatistic.Formula, customStatistic.Status, customStatistic.UserId));
        }

        [Authorize]
        [HttpDelete("{customStatisticId}")]
        public async Task<IActionResult> DeleteCustomStatistic(string userId, int customStatisticId)
        {

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound();

            var customStatistic = await _context.CustomStatistics
                .Include(s => s.Statistics)
                .SingleOrDefaultAsync(r => r.Id == customStatisticId && r.User.Id == userId);
            if (customStatistic == null)
                return NotFound();

            if (!HttpContext.User.IsInRole(ForumRoles.Regular)
                || HttpContext.User.FindFirstValue(JwtRegisteredClaimNames.Sub) != userId)
                return Forbid();

            _context.CustomStatistics.Remove(customStatistic);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("{customStatisticId}/algorithmImpressions")]
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

        [HttpGet("{customStatisticId}/algorithmImpressions/{impressionId}")]
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

        [HttpPost("{customStatisticId}/algorithmImpressions")]
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

        [HttpPut("{customStatisticId}/algorithmImpressions/{impressionId}")]
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

        [HttpDelete("{customStatisticId}/algorithmImpressions/{impressionId}")]
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
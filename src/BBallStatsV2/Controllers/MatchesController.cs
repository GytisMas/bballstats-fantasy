﻿using BBallStats.Data;
using BBallStatsV2.Data.Entities;
using BBallStatsV2.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using static BBallStats.Shared.Utils.DTOs;

namespace BBallStatsV2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MatchesController : ControllerBase
    {
        private readonly ForumDbContext _context;

        public MatchesController(ForumDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        public async Task<ActionResult<MatchDto>> GetLatestSeasonMatch(int seasonCode)
        {
            var match = await _context.Matches
                .Where(m => m.SeasonId == seasonCode)
                .OrderByDescending(m => m.GameId)
                .FirstOrDefaultAsync();

            if (match == null)
            {
                return NotFound();
            }

            return new MatchDto(match.GameId, match.SeasonId, match.HomeTeamId, match.AwayTeamId, match.MatchDate);
        }

        [HttpGet("unused/{seasonCode}")]
        public async Task<ActionResult<int>> GetOldestUnusedMatchId(int seasonCode, bool ignoreExisting)
        {
            var matches = await _context.Matches
                .Where(m => m.SeasonId == seasonCode)
                .Where(m => ignoreExisting || m.UsedInFantasy)
                .OrderBy(m => m.GameId)
                .ToListAsync();

            if (matches == null || matches.Count == 0)
            {
                return Ok(1);
            }

            int maxId = matches.Last().GameId;
            int unusedGameId = maxId + 1;

            for (int i = 1; i <= matches.Count(); i++)
            {
                if (matches[i-1].GameId != i) {
                    unusedGameId = i;
                    break;
                }
            }

            return Ok(unusedGameId);
        }

        [HttpPost]
        public async Task<ActionResult<MatchDto>> CreateOrGetMatch(MatchDto dto)
        {
            var matches = await _context.Matches
                .Where(m => m.GameId == dto.gameCode)
                .Where(m => m.SeasonId == dto.seasonCode)
                .FirstOrDefaultAsync();

            if (matches != null)
            {
                return Ok(dto);
            }

            var match = new Match()
            {
                GameId = dto.gameCode,
                SeasonId = dto.seasonCode,
                MatchDate = dto.MatchDate,
                HomeTeamId = dto.HomeTeamId,
                AwayTeamId = dto.AwayTeamId
            };

            _context.Matches.Add(match);
            await _context.SaveChangesAsync();

            return Created("/api/matches", dto);
        }

        [HttpGet("~/api/Participants/{participantId}/[controller]")]
        public async Task<ActionResult<IEnumerable<MatchPlayerDto>>> GetParticipantNextMatches(int participantId)
        {
            var participant = await _context.LeagueParticipants
                .Include(p => p.League)
                .Include(p => p.Team)
                    .ThenInclude(tp => tp.LeagueAvailablePlayer.Player)
                .SingleOrDefaultAsync(p => p.Id == participantId);

            if (participant == null)
            {
                return NotFound();
            }

            var selectedMatches = new Dictionary<string, MatchPlayerDto>();

            foreach (var player in participant.Team)
            {
                string playerId = player.LeagueAvailablePlayer.PlayerId;
                string teamId = player.LeagueAvailablePlayer.Player.CurrentTeamId;
                if (!selectedMatches.ContainsKey(teamId))
                {
                    var nextMatch = await _context.Matches
                            .Where(m => !m.UsedInFantasy
                                && m.MatchDate >= participant.League.StartDate && m.MatchDate <= participant.League.EndDate
                                && (m.AwayTeamId.Equals(teamId) || m.HomeTeamId.Equals(teamId))
                                )
                            .OrderBy(m => m.MatchDate)
                            .FirstOrDefaultAsync();
                    if (nextMatch == null)
                        continue;
                    selectedMatches.Add(player.LeagueAvailablePlayer.Player.CurrentTeamId,
                        new MatchPlayerDto(
                            new MatchDto(nextMatch.GameId, nextMatch.SeasonId, nextMatch.HomeTeamId, nextMatch.AwayTeamId, nextMatch.MatchDate),
                            new List<string> { playerId })
                    );

                    continue;
                }

                selectedMatches[teamId].PlayerIds.Add(playerId);
            }

            return selectedMatches.Values;
        }
    }
}

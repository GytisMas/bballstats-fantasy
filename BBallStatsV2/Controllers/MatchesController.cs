using BBallStats.Data;
using BBallStatsV2.Data.Entities;
using BBallStatsV2.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

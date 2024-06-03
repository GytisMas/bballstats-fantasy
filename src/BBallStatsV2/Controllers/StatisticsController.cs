using BBallStats.Data;
using BBallStats.Data.Entities;
using BBallStats.Shared;
using BBallStats2.Auth.Model;
using BBallStatsV2.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mono.TextTemplating;
using static BBallStats.Data.Entities.RegularStatistic;
using static BBallStats.Data.Entities.Statistic;

namespace BBallStatsV2.Controllers
{
    [Route("api/[controller]/")]
    [ApiController]
    public class StatisticsController : ControllerBase
    {
        private readonly ForumDbContext _context;
        
        public StatisticsController(ForumDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RegularStatisticDto>>> GetStatistics()
        {
            return await _context.RegularStatistics
                .Select(stat => new RegularStatisticDto(stat.Id, stat.Name, stat.DisplayName, stat.Status, stat.DefaultLeaguePointsPerStat, stat.DefaultIsChecked))
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<RegularStatisticDto>>> GetStatistic(int id)
        {
            var stat = await _context.RegularStatistics
                .FindAsync(id);

            if (stat == null)
                return NotFound();

            return Ok(new RegularStatisticDto(stat.Id, stat.Name, stat.DisplayName, stat.Status, stat.DefaultLeaguePointsPerStat, stat.DefaultIsChecked));
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<RegularStatisticDto>> CreateStatistic(CreateRegularStatisticDto dto)
        {
            if (!HttpContext.User.IsInRole(ForumRoles.Curator))
                return Forbid();

            var stat = new RegularStatistic()
            {
                Name = dto.Name,
                DisplayName = dto.DisplayName,
                Status = dto.Status,
                DefaultLeaguePointsPerStat = dto.DefaultLeaguePointsPerStat,
                DefaultIsChecked = dto.DefaultIsChecked
            };

            _context.RegularStatistics.Add(stat);
            await _context.SaveChangesAsync();

            return Ok(new RegularStatisticDto(stat.Id, stat.Name, stat.DisplayName, stat.Status, stat.DefaultLeaguePointsPerStat, stat.DefaultIsChecked));
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult<RegularStatisticDto>> UpdateStatistic(int id, RegularStatisticDto dto)
        {
            var stat = await _context.Statistics.FirstOrDefaultAsync(u => u.Id == id);
            if (stat == null)
                return NotFound();

            if (!HttpContext.User.IsInRole(ForumRoles.Curator))
                return Forbid();

            stat.Name = dto.Name;
            stat.DisplayName = dto.DisplayName;
            stat.Status = dto.Status;
            stat.DefaultLeaguePointsPerStat = dto.DefaultLeaguePointsPerStat;
            stat.DefaultIsChecked = dto.DefaultIsChecked;

            _context.Statistics.Update(stat);
            await _context.SaveChangesAsync();

            return Ok(new RegularStatisticDto(stat.Id, stat.Name, stat.DisplayName, stat.Status, stat.DefaultLeaguePointsPerStat, stat.DefaultIsChecked));
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteStatistic(int id)
        {
            var stat = await _context.Statistics.FirstOrDefaultAsync(u => u.Id == id);
            if (stat == null)
                return NotFound();

            if (!HttpContext.User.IsInRole(ForumRoles.Curator))
                return Forbid();

            _context.Statistics.Remove(stat);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/Leagues
        [HttpPost("UpdateStats")]
        public async Task<IActionResult> UpdateStats(StatSheet playerStatData)
        {
            var teams = await _context.Teams
                .Where(x => x.Id.Equals(playerStatData.LocalClubId)
                || x.Id.Equals(playerStatData.RoadClubId))
                .ToListAsync();
            if (teams.Find(x => x.Id.Equals(playerStatData.LocalClubId)) == null)
            {
                var newTeam = new Team() { Id = playerStatData.LocalClubId, Name = playerStatData.LocalClubName };
                teams.Add(newTeam);
                await _context.AddAsync(newTeam);
            }
            if (teams.Find(x => x.Id.Equals(playerStatData.RoadClubId)) == null)
            {
                var newTeam = new Team() { Id = playerStatData.RoadClubId, Name = playerStatData.RoadClubName };
                teams.Add(newTeam);
                await _context.AddAsync(newTeam);
            }


            List<string> playerCodes = playerStatData.PlayerInfo.Select(x => x.PlayerCode).Distinct().ToList();
            var relevantPlayers = await _context.Players
                .Include(x => x.PlayerStatistics)
                .ThenInclude(x => x.Statistic)
                .Where(x => playerCodes.Contains(x.Id))
                .ToListAsync();
            var statistics = await _context.RegularStatistics
                .ToListAsync();

            var StatTypes = playerStatData.GameStats.Select(x => x.StatName).Distinct().ToList();

            var newStatistics = CreateUnusedStatistics(statistics, StatTypes);
            await _context.Statistics.AddRangeAsync(newStatistics);
            statistics.AddRange(newStatistics);

            int i = 1;
            foreach (var code in playerCodes)            
            {
                bool addPlayer = false;
                var currentPlayer = relevantPlayers.Find(x => x.Id.Equals(code));

                var PlayerInfoEntries = playerStatData.PlayerInfo.Where(x => x.PlayerCode.Equals(code)).ToList();
                var PlayerStatEntries = playerStatData.GameStats.Where(x => x.PlayerCode.Equals(code)).ToList();

                if (currentPlayer == null)
                {
                    addPlayer = true;
                    currentPlayer = new Player()
                    {
                        Id = code,
                        Name = PlayerInfoEntries.FirstOrDefault(x => x.StatName.Equals("PlayerName")).StringVal,
                        CurrentTeamId = PlayerInfoEntries.FirstOrDefault(x => x.StatName.Equals("TeamCode")).StringVal,
                    };
                }

                bool emptyStats = !currentPlayer.PlayerStatistics.Any(ps => ps.Statistic.Name == "StartFive");
                if (addPlayer || emptyStats)
                {
                    currentPlayer = CreateUnusedPlayerStatistics(currentPlayer, statistics, PlayerStatEntries);
                }

                currentPlayer = UpdatePlayerStatistics(currentPlayer, PlayerStatEntries);

                if (addPlayer)
                    _context.Players.Add(currentPlayer);
                else
                    _context.Players.Update(currentPlayer);
            }
            await _context.SaveChangesAsync();
            return Ok();
        }

        private Player UpdatePlayerStatistics(Player player, List<PlayerStat> StatTypes)
        {
            foreach (var s in StatTypes)
            {
                var playerStat = player.PlayerStatistics.FirstOrDefault(x => x.Statistic.Name == s.StatName);
                if (playerStat != null)
                {
                    switch (s.StatPropertyType)
                    {
                        case "int":
                            playerStat.Value += (int)s.IntVal;
                            break;
                        case "int[]":
                            playerStat.Value += (int)s.IntArrVal[0];
                            playerStat.AttemptValue += (int)s.IntArrVal[1];
                            break;
                        case "bool":
                            if (s.Boolval != null && s.Boolval == true)
                                playerStat.Value++;
                            playerStat.AttemptValue++;
                            break;
                    }
                    playerStat.GameCount++;
                }
            }
            return player;
        }

        private Player CreateUnusedPlayerStatistics(Player player, List<RegularStatistic> stats, List<PlayerStat> StatTypes)
        {
            foreach (var s in StatTypes)
                player.PlayerStatistics.Add(new PlayerStatistic()
                {
                    Statistic = stats.Find(x => x.Name == s.StatName)
                });
            return player;
        }

        private List<RegularStatistic> CreateUnusedStatistics(List<RegularStatistic> existingStats, List<string> StatTypes) 
        {
            var statistics = new List<RegularStatistic>();
            foreach (var s in StatTypes)
                statistics = CheckOrAddStat(statistics, existingStats, s);
            return statistics;
        }

        private List<RegularStatistic> CheckOrAddStat(List<RegularStatistic> newStats, List<RegularStatistic> existingStats, string statName)
        {
            var a = existingStats.Find(x => x.Name == statName);
            if (a == null)
                newStats.Add(new RegularStatistic()
                {
                    Name = statName,
                    Status = Visibility.Main
                });
            return newStats;
        }
    }
}

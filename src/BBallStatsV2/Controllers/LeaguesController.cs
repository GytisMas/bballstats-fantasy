﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BBallStats.Data;
using BBallStatsV2.Data.Entities;
using BBallStatsV2.DTOs;
using BBallStats.Data.Entities;
using BBallStats.Shared;
using BBallStatsV2.DataStructures;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;

namespace BBallStatsV2.Controllers
{
    [Route("api/fantasy/leagues")]
    [ApiController]
    public class LeaguesController : ControllerBase
    {
        private readonly ForumDbContext _context;

        public LeaguesController(ForumDbContext context)
        {
            _context = context;
        }

        // GET: api/Leagues
        [HttpGet]
        public async Task<ActionResult<PagedListDto<ListedLeagueDto>>> GetPublicLeagues(string? nameFilter, bool activeOnly = false, 
            LeagueSortParameter sortBy = LeagueSortParameter.StartDateD, int pageIndex = 1, int pageSize = 10)
        {
            pageSize = PagedListData.FilteredPageSize(pageSize);
            pageIndex = PagedListData.FilteredPageIndex(pageIndex);
            int totalCount = (await _context.Leagues
                .Where(l => l.Password == null && (nameFilter == null || nameFilter == "" || l.Name.Contains(nameFilter))
                    && (!activeOnly || DateTime.UtcNow >= l.StartDate && DateTime.UtcNow <= l.EndDate && !l.IsOver))
                .CountAsync());
            int pageCount = totalCount / pageSize;
            if (totalCount % pageSize != 0)
                pageCount++;


            var leagues = _context.Leagues
                .Include(l => l.LeagueTemplate)
                .Include(l => l.LeagueHost)
                .Include(l => l.LeagueAvailablePlayers)
                .ThenInclude(lp => lp.Player)
                .ThenInclude(p => p.CurrentTeam)
                .Where(l => l.Password == null && (nameFilter == null || nameFilter == "" || l.Name.Contains(nameFilter))
                    && (!activeOnly || DateTime.UtcNow >= l.StartDate && DateTime.UtcNow <= l.EndDate && !l.IsOver));
                ;

            switch (sortBy)
            {
                case LeagueSortParameter.StartDate:
                    leagues = leagues.OrderBy(l => l.StartDate);
                    break;
                case LeagueSortParameter.StartDateD:
                    leagues = leagues.OrderByDescending(l => l.StartDate);
                    break;
                case LeagueSortParameter.EndDate:
                    leagues = leagues.OrderBy(l => l.EndDate);
                    break;
                case LeagueSortParameter.EndDateD:
                    leagues = leagues.OrderByDescending(l => l.EndDate);
                    break;
                case LeagueSortParameter.Name:
                    leagues = leagues.OrderBy(l => l.Name);
                    break;
                case LeagueSortParameter.NameD:
                    leagues = leagues.OrderByDescending(l => l.Name);
                    break;
            }
                
            leagues = leagues
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()                
                ;

            return new PagedListDto<ListedLeagueDto>(await leagues
                .Select(l => new ListedLeagueDto(l.Id, l.Name, l.EntryFee, l.IsActive, l.StartDate, l.EndDate,
                    l.LeagueTemplate.Name, l.LeagueHost.UserName,
                    l.LeagueAvailablePlayers.Select(lp => lp.Player.CurrentTeam.Name).Distinct().ToArray()))
                .ToListAsync(), pageIndex, pageCount);
        }
                
        [HttpGet("{leagueId}")]
        public async Task<ActionResult<LeagueWithParticipantsDto>> GetLeague(int leagueId)
        {
            var league = await _context.Leagues
                .Include(l => l.Participants)
                .ThenInclude(p => p.User)
                .FirstOrDefaultAsync(l => l.Id == leagueId);

            if (league == null)
            {
                return NotFound();
            }

            int? participantId = null;
            if (HttpContext.User.Identity?.IsAuthenticated ?? false)
            {
                var userId = HttpContext.User.FindFirstValue(JwtRegisteredClaimNames.Sub);
                var userParticipantId = league.Participants
                    .FirstOrDefault(p => p.UserId.Equals(userId))?.Id ?? null;
                if (userParticipantId != null)
                    participantId = userParticipantId;
            }

            league.Participants = league.Participants.OrderByDescending(p => p.Points).ThenBy(p => p.EntryDate).ToList();


            return new LeagueWithParticipantsDto(league.Id, league.Name, !league.HasStarted, participantId, league.EntryFee, league.CreationDate, league.StartDate, 
                league.EndDate, league.Participants.Select(p => new ParticipantDto(p.Id, p.TeamName, p.User.UserName, p.Points)).ToArray());
        }

        [HttpGet("{leagueId}/Players")]
        public async Task<ActionResult<LeaguePlayersDto>> GetLeagueWithPlayers(int leagueId)
        {
            var league = await _context.Leagues
                .Include(l => l.LeagueAvailablePlayers)
                .ThenInclude(lp => lp.Player)
                .FirstOrDefaultAsync(l => l.Id == leagueId);

            if (league == null)
            {
                return NotFound();
            }

            var players = league.LeagueAvailablePlayers
                .Select(p => new PlayerInLeagueDto(p.PlayerId, p.Id, p.Player.Name, (int)p.Player.Role, p.Player.CurrentTeamId, p.Price))
                .ToList();
            
            return new LeaguePlayersDto(league.Name, league.IsActive, league.EntryFee, league.LeagueTemplateId, league.Password != null, players);
        }

        [HttpPost("{leagueId}/Password")]
        public async Task<ActionResult<bool>> CheckLeaguePassword(int leagueId, string Password)
        {
            var league = await _context.Leagues
                .FindAsync(leagueId);

            if (league == null)
            {
                return NotFound();
            }

            if (league.Password == null)
            {
                return UnprocessableEntity("League is not private");
            }

            return Ok(league.Password == Password);
        }

        [Authorize]
        [HttpGet("{leagueId}/user/")]
        public async Task<ActionResult<int?>> GetUserParticipantId(int leagueId)
        {
            var league = await _context.Leagues
                .Include(l => l.Participants)
                .SingleOrDefaultAsync(l => l.Id == leagueId);

            if (league == null)
            {
                return NotFound();
            }

            var userId = HttpContext.User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            var user = await _context.Users
                .FindAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            int? participantId = league.Participants.FirstOrDefault(p => p.UserId.Equals(userId))?.Id ?? null;

            return Ok(participantId);
        }

        [HttpGet("{leagueId}/participants/{participantId}")]
        public async Task<ActionResult<LeagueParticipant>> GetParticipant(int leagueId, int participantId)
        {
            var league = await _context.Leagues.FindAsync(leagueId);
            if (league == null)
            {
                return NotFound();
            }

            var participant = await _context.LeagueParticipants
                .Include(p => p.User)
                .Include(p => p.Team)
                    .ThenInclude(t => t.LeagueAvailablePlayer)
                        .ThenInclude(lp => lp.Player)
                            .ThenInclude(plr => plr.CurrentTeam)
                .Include(p => p.Team)
                    .ThenInclude(t => t.LeaguePlayerRole)
                .FirstOrDefaultAsync(p => p.Id == participantId && p.LeagueId == leagueId);

            if (participant == null)
            {
                return NotFound();
            }

            bool participantIsUser = false;
            if (HttpContext.User.Identity?.IsAuthenticated ?? false)
            {
                var userId = HttpContext.User.FindFirstValue(JwtRegisteredClaimNames.Sub);
                var userParticipantId = league.Participants
                    .FirstOrDefault(p => p.UserId.Equals(userId))?.Id ?? null;
                if (userParticipantId != null)
                    participantIsUser = true;
            }

            bool allowRosterChanges = league.IsActive && DateTime.UtcNow.Hour < 12;

            return Ok(new ParticipantWithTeamDto(league.IsActive, allowRosterChanges, participant.Id, participantIsUser, participant.TeamName, participant.User.UserName, participant.Points, 
                participant.Team.Select(t => new ParticipantPlayerInfoDto(t.Id, t.Points, t.PointsLastGame, t.LeagueAvailablePlayer.Player.Id, t.LeagueAvailablePlayer.Player.Name, 
                t.LeagueAvailablePlayer.Player.CurrentTeam.Name, t.LeagueAvailablePlayer.Price, t.LeaguePlayerRoleId, t.LeaguePlayerRole.Name, t.LeaguePlayerRole.RoleToReplaceId)).ToArray()
                ));
        }

        [HttpPut("{leagueId}/participants/{participantId}/roles")]
        public async Task<ActionResult<bool>> UpdateParticipantRoles(int leagueId, int participantId, ParticipantRoleChangeDto participantRoleChangeDto)
        {
            var league = await _context.Leagues
                .Include(l => l.Participants)
                .FirstOrDefaultAsync(l => l.Id == leagueId);
            if (league == null)
            {
                return NotFound();
            }

            var participant = await _context.LeagueParticipants
                .Include(p => p.Team)
                    .ThenInclude(t => t.LeaguePlayerRole)
                .FirstOrDefaultAsync(p => p.Id == participantId && p.LeagueId == leagueId);

            if (participant == null)
            {
                return NotFound();
            }

            foreach (var leagueParticipant in league.Participants)
            {
                if (leagueParticipant.TeamName.Equals(participantRoleChangeDto.TeamName) 
                    && leagueParticipant.Id != participant.Id)
                    return BadRequest("Team with this name already exists.");
            }

            participant.TeamName = participantRoleChangeDto.TeamName;

            foreach (var usedPlayer in participant.Team)
            {
                ParticipantPlayerRoleDto? newRole = participantRoleChangeDto.PlayerRolePairs.FirstOrDefault(prp => prp.Id == usedPlayer.Id);
                if (newRole == null)
                    return BadRequest("Invalid used player leagueId");
                usedPlayer.LeaguePlayerRoleId = newRole.RoleId;
            }
            _context.LeagueParticipants.Update(participant);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("{leagueId}/participants")]
        public async Task<ActionResult<LeagueParticipant>> GetParticipants(int leagueId)
        {
            var participants = await _context.LeagueParticipants.Where(p => p.LeagueId == leagueId).ToListAsync();
            if (participants == null)
            {
                return NotFound();
            }

            return Ok(participants);
        }

        [HttpPost("{leagueId}/participants/")]
        [Authorize]
        public async Task<ActionResult<int>> CreateParticipant(int leagueId, CreateParticipantDto createParticipantDto)
        {
            var userId = HttpContext.User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            var user = await _context.Users
                .FindAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var league = await _context.Leagues
                .Include(x => x.LeagueAvailablePlayers)
                .Include(x => x.LeagueTemplate)
                .ThenInclude(x => x.Roles)
                .FirstOrDefaultAsync(x => x.Id == leagueId);
            if (league == null)
            {
                return NotFound();
            }

            var userBalance = await _context.Transactions
                .Where(t => t.SenderId == userId || t.RecipientId == userId)
                .SumAsync(t => t.RecipientId == userId ? t.Amount : -t.Amount);
            var playersPrice = await _context.LeagueAvailablePlayers
                .Where(lp => createParticipantDto.Players.Select(p => p.LeagueAvailablePlayerId).Contains(lp.Id))
                .Select(lp => lp.Price)
                .SumAsync();

            if (userBalance < playersPrice + league.EntryFee)
            {
                return UnprocessableEntity("InsufficientFunds");
            }

            var participant = new LeagueParticipant()
            {
                EntryDate = DateTime.UtcNow,
                TeamName = createParticipantDto.TeamName,
                League = league,
                User = user,
                Points = 0
            };

            participant.Team = new List<ParticipantsRosterPlayer>();
            // validation
            foreach (var player in createParticipantDto.Players)
            {
                LeaguePlayerRole? role;
                if ((role = league
                    .LeagueTemplate.Roles
                    .FirstOrDefault(x => x.Id == player.LeaguePlayerRoleId)) == null)
                    return BadRequest("Invalid player role");
                LeagueAvailablePlayer? availablePlayer;
                if ((availablePlayer = league
                    .LeagueAvailablePlayers
                    .FirstOrDefault(x => x.Id == player.LeagueAvailablePlayerId)) == null)
                    return BadRequest("Invalid player");
                participant.Team.Add(new ParticipantsRosterPlayer()
                {
                    LeaguePlayerRole = role,
                    LeagueParticipant = participant,
                    LeagueAvailablePlayer = availablePlayer
                });
            }

            var newTransactions = new List<Transaction> 
            {
                new Transaction
                {
                    TransactionType = TransactionType.LeagueEntry,
                    Amount = league.EntryFee,
                    Sender = participant.User,
                    RecipientId = league.LeagueHostId,
                    Date = DateTime.UtcNow
                }, 
                new Transaction
                {
                    TransactionType = TransactionType.LeagueEntry,
                    Amount = playersPrice,
                    Sender = participant.User,
                    RecipientId = league.LeagueHostId,
                    Date = DateTime.UtcNow
                }
            };

            _context.Transactions.AddRange(newTransactions);
            _context.LeagueParticipants.Add(participant);
            await _context.SaveChangesAsync();
            return Ok(participant.Id);
        }

        [HttpPut("{leagueId}/participants/{participantId}")]
        public async Task<ActionResult<LeagueParticipant>> UpdateParticipant(int leagueId, int participantId, CreateParticipantDto createParticipantDto)
        {
            var league = await _context.Leagues
                .Include(x => x.LeagueAvailablePlayers)
                .Include(x => x.LeagueTemplate)
                    .ThenInclude(x => x.Roles)
                .Include(l => l.Participants)
                .FirstOrDefaultAsync(l => l.Id == leagueId);
            if (league == null)
            {
                return NotFound();
            }

            var participant = await _context.LeagueParticipants
                .Include(p => p.Team)
                    .ThenInclude(t => t.LeaguePlayerRole)
                .FirstOrDefaultAsync(p => p.Id == participantId && p.LeagueId == leagueId);

            if (participant == null)
            {
                return NotFound();
            }
            

            participant.Team = new List<ParticipantsRosterPlayer>();
            // validation
            foreach (var player in createParticipantDto.Players)
            {
                LeaguePlayerRole? role;
                if ((role = league
                    .LeagueTemplate.Roles
                    .FirstOrDefault(x => x.Id == player.LeaguePlayerRoleId)) == null)
                    return BadRequest("Invalid player role");
                LeagueAvailablePlayer? availablePlayer;
                if ((availablePlayer = league
                    .LeagueAvailablePlayers
                    .FirstOrDefault(x => x.Id == player.LeagueAvailablePlayerId)) == null)
                    return BadRequest("Invalid player");
                participant.Team.Add(new ParticipantsRosterPlayer()
                {
                    LeaguePlayerRole = role,
                    LeagueParticipant = participant,
                    LeagueAvailablePlayer = availablePlayer
                });
            }

            _context.LeagueParticipants.Update(participant);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{leagueId}/participants/{participantId}")]
        [Authorize]
        public async Task<IActionResult> DeleteParticipant(int leagueId, int participantId)
        {
            var league = await _context.Leagues.FindAsync(leagueId);
            if (league == null)
            {
                return NotFound();
            }

            if (league.IsActive)
            {
                return UnprocessableEntity("League is already active");
            }

            var userId = HttpContext.User.FindFirstValue(JwtRegisteredClaimNames.Sub);

            var participant = await _context.LeagueParticipants.FindAsync(participantId);
            if (participant == null)
            {
                return NotFound();
            }

            if (!participant.UserId.Equals(userId) && !participant.UserId.Equals(league.LeagueHostId))
            {
                return UnprocessableEntity("Participant is not host nor authorized user");
            }

            _context.LeagueParticipants.Remove(participant);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPatch("{leagueId}")]
        public async Task<IActionResult> PatchLeague(int leagueId, PatchLeagueDto patchLeagueDto)
        {
            var league = await _context.Leagues.FindAsync(leagueId);
            if (league == null)
            {
                return NotFound("League not found");
            }
            var leagueTemplate = await _context.LeagueTemplates.FindAsync(patchLeagueDto.LeagueTemplateId);
            if (leagueTemplate == null)
            {
                return NotFound("League template not found");
            }
            var leagueHost = await _context.Users.FindAsync(patchLeagueDto.LeagueTemplateId);
            if (leagueHost == null)
            {
                return NotFound("User not found");
            }

            List<Player> players = patchLeagueDto.AvailablePlayerIds != null
                ? await _context.Players.Where(x => patchLeagueDto.AvailablePlayerIds.Contains(x.Id)).ToListAsync()
                : await _context.Players.ToListAsync();
            if (players == null)
            {
                return NotFound("Players not found");
            }

            league.Name = patchLeagueDto.Name;
            league.EntryFee = patchLeagueDto.EntryFee;
            league.CreationDate = DateTime.Now;
            league.StartDate = patchLeagueDto.StartDate;
            league.EndDate = patchLeagueDto.EndDate;
            league.LeagueTemplateId = leagueTemplate.Id;
            league.LeagueHost = leagueHost;
            league.LeagueAvailablePlayers = players
                .Select(x => new LeagueAvailablePlayer()
                {
                    Price = 500,
                    League = league,
                    PlayerId = x.Id
                }).ToList();

            _context.Leagues.Update(league);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LeagueExists(leagueId))
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

        [HttpPost]
        public async Task<ActionResult<League>> CreateLeague(PostLeagueDto postLeagueDto)
        {
            var dateDifference = (postLeagueDto.EndDate - postLeagueDto.StartDate).Days;
            if (dateDifference > 30 || dateDifference < 1)
            {
                return UnprocessableEntity("Invalid date values");
            }

            if (postLeagueDto.AvailablePlayerTeamIds == null || postLeagueDto.AvailablePlayerTeamIds.Length == 0)
            {
                return UnprocessableEntity("No scheduled matches during league time period.");
            }

            var leagueTemplate = await _context.LeagueTemplates
                .FindAsync(postLeagueDto.LeagueTemplateId);

            if (leagueTemplate == null)
            {
                return NotFound("League template not found");
            }
            var leagueHost = await _context.Users.FindAsync(postLeagueDto.LeagueHostId);
            if (leagueHost == null)
            {
                return NotFound("User not found");
            }

            List<Player> players = postLeagueDto.AvailablePlayerTeamIds != null
                ? await _context.Players.Where(x => postLeagueDto.AvailablePlayerTeamIds.Contains(x.CurrentTeamId)).ToListAsync()
                : await _context.Players.ToListAsync();

            if (players == null)
            {
                return NotFound("Players not found");
            }

            League league = new League()
            {
                Name = postLeagueDto.Name,
                EntryFee = postLeagueDto.EntryFee,
                CreationDate = DateTime.UtcNow,
                StartDate = postLeagueDto.StartDate,
                EndDate = postLeagueDto.EndDate,
                LeagueTemplateId = leagueTemplate.Id,
                LeagueHost = leagueHost,
            };

            if (postLeagueDto.Password != null && postLeagueDto.Password != "")
                league.Password = postLeagueDto.Password;

            league.Payments = new List<LeaguePayment>();
            int placement = 1;
            foreach (var amount in postLeagueDto.LeaguePayments)
            {
                league.Payments.Add(
                    new LeaguePayment
                    {
                        Placing = placement++,
                        Amount = amount
                    }
                );
            }

            var statistics = await _context.Statistics.Where(s => s.Status == Visibility.Main).ToListAsync();
            league.LeagueAvailablePlayers = players
                .Select(x => new LeagueAvailablePlayer()
                {
                    League = league,
                    PlayerId = x.Id,
                    Price = PlayerDefaultPrice(x.Id, statistics).Result
                }).ToList();

            _context.Leagues.Add(league);
            await _context.SaveChangesAsync();

            return Ok(new LeagueDto(league.Id, league.EntryFee, league.CreationDate, league.StartDate, league.EndDate, league.LeagueTemplateId, league.LeagueHostId));
        }

        [HttpPost("results")]
        public async Task<IActionResult> UpdateLeaguePoints(StatSheet playerStatData)
        {
            List<string> InactivePlayers = new List<string>();
            var Players = await _context.Players.ToListAsync();

            foreach (var player in Players)
            {
                bool notInRelevantTeam = false;
                if (!player.CurrentTeamId.Equals(playerStatData.LocalClubId) &&
                    !player.CurrentTeamId.Equals(playerStatData.RoadClubId))
                    notInRelevantTeam = true;
                if (!notInRelevantTeam)
                {
                    var playerInfo = playerStatData.GameStats.FirstOrDefault(
                        stat => stat.PlayerCode.Equals(player.Id)
                        && stat.StatName.Equals("TimePlayedSeconds"));
                    if (playerInfo == null || playerInfo.IntVal < 60)
                        player.SkippedLastGame = true;
                    else
                        player.SkippedLastGame = false;
                }

                if (player.SkippedLastGame)
                    InactivePlayers.Add(player.Id);
            }
            _context.Players.UpdateRange(Players);

            var statistics = await _context.RegularStatistics
            .ToListAsync();

            var PlayerGameStats = new Dictionary<string, PlayerGameStatsDto>();
            foreach (var playerInfo in Players.Select(x => new {Id = x.Id, TeamId = x.CurrentTeamId } ))
            {
                PlayerGameStats.Add(playerInfo.Id,
                    new PlayerGameStatsDto(playerInfo.TeamId,
                        playerStatData.GameStats
                            .Where(x => x.PlayerCode.Equals(playerInfo))
                            .Select(x => new PlayerGameStatDto(
                                statistics.First(y => y.Name.Equals(x.StatName)).Id,
                                x.IntVal != null ? (int)x.IntVal :
                                    x.IntArrVal != null ? x.IntArrVal[0] :
                                    x.Boolval != null ? 1 : WarnVal()
                                )
                            )
                            .ToList()
                    )
                );
            }

            var usedPlayers = await _context.ParticipantsRosterPlayers
                .Include(prp => prp.LeaguePlayerRole.Statistics)
                .Include(prp => prp.LeagueParticipant.League.LeagueTemplate)
                .Include(prp => prp.LeagueAvailablePlayer)
                .Where(prp => 
                    !prp.LeagueParticipant.League.IsOver 
                    && DateTime.UtcNow >= prp.LeagueParticipant.League.StartDate 
                    && DateTime.UtcNow <= prp.LeagueParticipant.League.EndDate
                    && 
                    (prp.LeagueAvailablePlayer.Player.CurrentTeamId.Equals(playerStatData.LocalClubId)
                    || prp.LeagueAvailablePlayer.Player.CurrentTeamId.Equals(playerStatData.RoadClubId)))
                .ToListAsync();

            Dictionary<string, double> PlayerRolePoints = new Dictionary<string, double>();
            foreach (var usedPlayer in usedPlayers)
            {
                double teamWinOrLosePoints =
                    PlayerGameStats[usedPlayer.LeagueAvailablePlayer.PlayerId].TeamId.Equals(playerStatData.WinnerClubId)
                    ? usedPlayer.LeagueParticipant.League.LeagueTemplate.TeamWinPoints
                    : usedPlayer.LeagueParticipant.League.LeagueTemplate.TeamLosePoints;

                usedPlayer.PointsLastGame = usedPlayer.Points;
                double? benchMultiplier = usedPlayer.LeagueParticipant.League.LeagueTemplate.BenchMultiplier;
                string currentDictKey = $"{usedPlayer.LeagueAvailablePlayer.PlayerId}_{usedPlayer.LeaguePlayerRoleId}";

                LeaguePlayerRole currentRole = usedPlayer.LeaguePlayerRole;
                bool isStartFive = benchMultiplier == null ? true :
                    currentRole.RoleToReplaceId == null || IsPlayerReplacement(usedPlayer, InactivePlayers);

                if (!isStartFive && benchMultiplier < 0.01)
                    continue;

                if (!PlayerRolePoints.ContainsKey(currentDictKey))
                {
                    double currentDictValue = 0;
                    foreach (var stat in PlayerGameStats[usedPlayer.LeagueAvailablePlayer.PlayerId].Stats)
                    {
                        // pakeist stat leagueId i name, pridet metoda custom stat evaluation
                        var statToCount = currentRole.Statistics.FirstOrDefault(x => x.StatisticId == stat.StatisticId);
                        if (statToCount == null)
                            continue;
                        currentDictValue += stat.value * statToCount.PointsPerStat;
                    }

                    PlayerRolePoints.Add(currentDictKey, currentDictValue);
                }
                var pointsToGive = PlayerRolePoints[currentDictKey];
                pointsToGive += teamWinOrLosePoints;

                if (!isStartFive && benchMultiplier != null)
                    pointsToGive = pointsToGive * (double)benchMultiplier;

                pointsToGive = Math.Round(pointsToGive, 2);
                usedPlayer.Points += pointsToGive;
                usedPlayer.LeagueParticipant.Points += pointsToGive;
            }
            _context.ParticipantsRosterPlayers.UpdateRange(usedPlayers);
            // ^^ for scaling tiesiog wrapping for'e ir pridet skip / take

            var match = await _context.Matches
                .FirstOrDefaultAsync(m => m.GameId == playerStatData.GameId && m.SeasonId == playerStatData.SeasonId);

            if (match != null)
            {
                match.UsedInFantasy = true;
                _context.Matches.Update(match);
            }

            await _context.SaveChangesAsync();

            // league baigima cia gal kviest

            return NoContent();

            #region old method
            //List<string> PlayerIds = playerStatData.PlayerInfo.Select(x => x.PlayerCode).Distinct().ToList();

            //PlayerIds.Sort();
            //var Players = await _context.Players.Where(x => PlayerIds.Contains(x.Id))
            //    .Include(player => player.LeagueAvailablePlayers)
            //        .ThenInclude(leaguePlayer => leaguePlayer.UsedPlayers)
            //            .ThenInclude(participantsPlayer => participantsPlayer.LeagueParticipant)
            //    .Include(player => player.LeagueAvailablePlayers) // galimai null error
            //        .ThenInclude(leaguePlayer => leaguePlayer.League)
            //            .ThenInclude(league => league.LeagueTemplate)
            //    //.Where(player => player.LeagueAvailablePlayers != null 
            //    //    && player.LeagueAvailablePlayers.All(leaguePlayer => leaguePlayer.League.IsActive))
            //    .Include(player => player.LeagueAvailablePlayers)
            //        .ThenInclude(leaguePlayer => leaguePlayer.UsedPlayers)
            //            .ThenInclude(participantsPlayer => participantsPlayer.LeaguePlayerRole)
            //                .ThenInclude(role => role.Statistics)
            //    .ToListAsync();


            //// 1. check for active leagues
            //// foreach (var player in Players)
            //// {
            ////     player.LeagueAvailablePlayers = player.LeagueAvailablePlayers.Where(x => x.League.IsActive).ToList();
            //// }

            // 2. no check
            //Players = Players.Where(player => player.LeagueAvailablePlayers.Count > 0).ToList();

            //Players.Sort((a, b) => a.Id.CompareTo(b.Id));

            //var statistics = await _context.RegularStatistics
            //    .ToListAsync();

            //var PlayerGameStats = new Dictionary<string, List<PlayerGameStatDto>>();
            //foreach (var leagueId in Players.Select(x => x.Id))
            //{
            //    PlayerGameStats.Add(leagueId,
            //        playerStatData.GameStats
            //            .Where(x => x.PlayerCode.Equals(leagueId))
            //            .Select(x => new PlayerGameStatDto(
            //                statistics.First(y => y.Name.Equals(x.StatName)).Id,
            //                x.IntVal != null ? (int)x.IntVal :
            //                    x.IntArrVal != null ? x.IntArrVal[0] :
            //                    x.Boolval != null ? 1 : WarnVal()
            //            )).ToList()
            //    );
            //}


            //for (int i = 0; i < Players.Count; i++)
            //{
            //    var player = Players[i];
            //    var PlayerStats = PlayerGameStats[player.Id];
            //    Dictionary<string, double> PlayerRolePoints = new Dictionary<string, double>();
            //    var usedPlayers = await _context.ParticipantsRosterPlayers

            //        .Where(prp => prp.LeagueParticipant.League.IsActive
            //            && prp.LeagueAvailablePlayer.PlayerId.Equals(player.Id))
            //        // for scaling tiesiog wrapping for'e ir pridet skip / take
            //        .ToListAsync();
            //    foreach (var participantPlayer in usedPlayers)
            //    {
            //        int? benchMultiplier = participantPlayer.LeagueParticipant.League.LeagueTemplate.BenchMultitplier;
            //        string currentDictKey = $"{player.Id}_{participantPlayer.LeaguePlayerRoleId}";
            //        LeaguePlayerRole currentRole = participantPlayer.LeaguePlayerRole;
            //        bool isStartFive = benchMultiplier != null && (bool)currentRole.StartFive;



            //        if (!PlayerRolePoints.ContainsKey(currentDictKey))
            //        {
            //            double currentDictValue = 0;
            //            foreach (var stat in PlayerStats.Stats)
            //            {
            //                // pakeist stat leagueId i name, pridet metoda custom stat evaluation
            //                var statToCount = currentRole.Statistics.FirstOrDefault(x => x.StatisticId == stat.StatisticId);
            //                if (statToCount == null)
            //                    continue;
            //                currentDictValue += stat.value * statToCount.PointsPerStat;
            //            }

            //            PlayerRolePoints.Add(currentDictKey, currentDictValue);
            //        }
            //        //foreach (var availablePlayer in player.LeagueAvailablePlayers)
            //        //{
            //        //    int? benchMultiplier = availablePlayer.League.LeagueTemplate.BenchMultitplier;
            //        //    foreach (var participantPlayer in availablePlayer.UsedPlayers)
            //        //    {
            //        //        string currentDictKey = $"{player.Id}_{participantPlayer.LeaguePlayerRoleId}";
            //        //        LeaguePlayerRole currentRole = participantPlayer.LeaguePlayerRole;
            //        //        bool isStartFive = benchMultiplier != null && (bool)currentRole.StartFive;



            //        //        if (!PlayerRolePoints.ContainsKey(currentDictKey))
            //        //        {
            //        //            double currentDictValue = 0;
            //        //            foreach (var stat in PlayerStats.Stats)
            //        //            {
            //        //                // pakeist stat leagueId i name, pridet metoda custom stat evaluation
            //        //                var statToCount = currentRole.Statistics.FirstOrDefault(x => x.StatisticId == stat.StatisticId);
            //        //                if (statToCount == null)
            //        //                    continue;
            //        //                currentDictValue += stat.value * statToCount.PointsPerStat;
            //        //            }

            //        //            PlayerRolePoints.Add(currentDictKey, currentDictValue);
            //        //        }

            //        //        participantPlayer.LeagueParticipant.Points += PlayerRolePoints[currentDictKey];
            //        //    }
            //        //}
            //        _context.Players.Update(player);
            //}
            #endregion
        }

        private bool IsPlayerReplacement(ParticipantsRosterPlayer usedPlayer, List<string> InactivePlayers)
        {
            int? replacementRoleId = usedPlayer.LeaguePlayerRole.RoleToReplaceId;
            if (replacementRoleId == null) { return false; }

            string potentialReplacementPlayerId = usedPlayer.LeagueParticipant.Team
                .FirstOrDefault(x => x.LeaguePlayerRoleId == replacementRoleId)?
                .LeagueAvailablePlayer.PlayerId ?? "notfound";

            return InactivePlayers.Contains(potentialReplacementPlayerId);
        }

        [HttpPost("endleague")]
        public async Task<IActionResult> EndInactiveLeagues()
        {
            var activeLeagueIds = await _context.Leagues
                .Where(l => !l.IsOver && DateTime.UtcNow >= l.EndDate)
                .Select(l => l.Id)
                .ToListAsync();

            if (activeLeagueIds == null || activeLeagueIds.Count == 0)
            {
                return NotFound();
            }

            var endLeagueTasks = new List<Task>();
            foreach (var leagueId in activeLeagueIds)
            {
                endLeagueTasks.Add(EndLeague(leagueId));
            }

            await Task.WhenAll(endLeagueTasks);

            return Ok();
        }

        [HttpPost("endleague/{leagueId}")]
        public async Task<IActionResult> EndLeague(int leagueId)
        {
            var league = await _context.Leagues
                .Include(l => l.Payments)
                .Include(l => l.Participants)
                .FirstOrDefaultAsync(l => l.Id == leagueId);

            if (league == null)
            {
                return NotFound();
            }

            league.Participants = league.Participants.OrderByDescending(p => p.Points).ThenBy(p => p.EntryDate).ToList();
            league.Payments = league.Payments.OrderBy(p => p.Placing).ToList();

            var Transactions = new List<Transaction>();
            for (int i = 0; i < league.Participants.Count; i++)
            {
                var participant = league.Participants[i];
                Transactions.Add(new Transaction
                {
                    TransactionType = TransactionType.LeaguePrizePayment,
                    Amount = league.Payments[i].Amount,
                    RecipientId = participant.UserId,
                    SenderId = league.LeagueHostId,
                    Date = DateTime.UtcNow
                });
            }

            if (league.Payments.Count > league.Participants.Count)
            {
                int remainingPayments = league.Payments
                    .Skip(league.Participants.Count)
                    .Sum(p => p.Amount);

                int amountPerParticipant = remainingPayments / league.Participants.Count;
                int amountForWinner = amountPerParticipant + (remainingPayments % league.Participants.Count);

                Transactions[0].Amount += amountForWinner;
                foreach (var transaction in Transactions.Skip(1))
                {
                    transaction.Amount += amountPerParticipant;
                }
            }

            league.IsOver = true;

            _context.Leagues.Update(league);
            await _context.Transactions.AddRangeAsync(Transactions);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{leagueId}")]
        public async Task<IActionResult> DeleteLeague(int leagueId)
        {
            var league = await _context.Leagues.FindAsync(leagueId);
            if (league == null)
            {
                return NotFound();
            }

            _context.Leagues.Remove(league);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("~/api/participants/byUser/{userId}")]
        public async Task<IEnumerable<LeagueParticipationDto>> GetActiveUserParticipations(string userId)
        {
            var participations = await _context.LeagueParticipants
                .Include(p => p.League)
                    .ThenInclude(l => l.Participants)
                .Where(p => p.UserId.Equals(userId)
                    && !p.League.IsOver
                )
                .ToListAsync();

            return participations.Select(p => new LeagueParticipationDto(p.Id, p.LeagueId, p.TeamName, p.League.Name,
                p.League.StartDate < DateTime.UtcNow ? p.League.ParticipantPlacement(p.Id) : -1));
        }

        private async Task<int> PlayerDefaultPrice(string playerId, List<Statistic> StatList)
        {
            double price = 0;
            var StatListIds = StatList.Select(x => x.Id).ToList();
            var playerStatistics = await _context.PlayerStatistics.Where(playerS => playerS.PlayerId.Equals(playerId) && StatListIds.Contains(playerS.StatisticId)).ToListAsync();
            foreach (var stat in StatList) 
            {
                var playerStat = playerStatistics.SingleOrDefault(ps => ps.StatisticId == stat.Id);
                if (playerStat == null)
                {
                    Console.WriteLine($"Player statistic not found: playerId: {playerId}, staticticId: {stat.Id}");
                    continue;
                }

                price += playerStat.Value * stat.DefaultLeaguePointsPerStat;
            }
            return (int)price;
        }

        private bool LeagueExists(int leagueId)
        {
            return _context.Leagues.Any(e => e.Id == leagueId);
        }

        private int WarnVal()
        {
            Console.WriteLine("Statistic value not found");
            return 1;
        }
    }
}
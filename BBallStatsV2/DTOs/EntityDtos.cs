using BBallStats.Data.Entities;
using BBallStatsV2.Data.Entities;

namespace BBallStatsV2.DTOs
{
    public record PostLeagueDto(string Name, int EntryFee, DateTime StartDate, 
        DateTime EndDate, int LeagueTemplateId, string LeagueHostId, string[]? AvailablePlayerTeamIds, int[] LeaguePayments, string? Password);
    public record PatchLeagueDto(string Name, int EntryFee, DateTime StartDate,
        DateTime EndDate, int LeagueTemplateId, string LeagueHostId, string[] AvailablePlayerIds);
    public record ParticipantsRosterPlayerDto(int LeaguePlayerRoleId, int LeagueAvailablePlayerId);
    public record CreateParticipantDto(string TeamName, ParticipantsRosterPlayerDto[] Players);
    public record LeagueStatDto(double PointsPerStat, int StatisticId);
    public record LeagueRoleDto(string Name, LeagueStatDto[] LeagueStats);
    public record LeagueRoleWithIdDto(int Id, string Name, int? RoleToReplaceIndex, LeagueStatDto[] LeagueStats);
    public record LeagueTemplateDto(string Name, double? BenchMultiplier, LeagueRoleWithIdDto[] LeagueRoles);
    public record LeaguePlayersDto(string Name, bool IsActive, int EntryFee, int LeagueTemplateId, bool IsPrivate, List<PlayerInLeagueDto> Players);
    public record PlayerGameStatsDto(string TeamId, List<PlayerGameStatDto> Stats);
    public record PlayerGameStatDto(int StatisticId, double value);
    public record ParticipantDto(int Id, string TeamName, string UserName, double Points);
    public record ParticipantWithTeamDto(bool LeagueIsActive, bool AllowRosterChanges, int Id, string TeamName, string UserName, double Points, ParticipantPlayerInfoDto[] Team);
    public record ParticipantPlayerInfoDto(int Id, double Points, double PointsLastGame, string PlayerId, string PlayerName, string TeamName, int Price, int RoleId, string RoleName, int? RoleToReplaceId);
    public record ParticipantRoleChangeDto(string TeamName, ParticipantPlayerRoleDto[] PlayerRolePairs);
    public record ParticipantPlayerRoleDto(int Id, int RoleId);
    public record LeagueWithParticipantsDto(int Id, string Name, bool NotStarted, int? UserParticipantId, int Entryfee, DateTime CreationDate, DateTime StartDate,
        DateTime EndDate, ParticipantDto[] Participants);
    public record PagedListDto<T>(List<T> Items, int PageIndex, int PageCount);

    public record UserDto(string Id, string Username, string Email, IList<string> roles);
    public record UserWithoutRolesDto(string Id, string Username, string Email);
    public record CreateUserDto(string Username, string Password, string Email, int Role);
    public record UpdateUserDto(string OldPassword, string NewPassword, string Email, int Role);

    public record MatchPlayerDto(MatchDto Match, List<string> PlayerIds);
    public record MatchDto(int gameCode, int seasonCode, string HomeTeamId, string AwayTeamId, DateTime MatchDate);
}

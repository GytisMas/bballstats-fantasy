using BBallStatsV2.Data.Entities;
using Microsoft.Extensions.Hosting;

namespace BBallStats.Data.Entities
{
	public enum PlayerRole
	{
		Guard = 0,
		Forward = 1,
		Center = 2,
		Other = 3,
    }

	public class Player
	{
		public string Id { get; set; }
		public required string Name { get; set; }
		public PlayerRole Role { get; set; }
		public string CurrentTeamId { get; set; }
		public Team CurrentTeam { get; set; }
        public bool SkippedLastGame { get; set; }
        public bool ForbidAutoUpdate { get; set; }
        public ICollection<LeagueAvailablePlayer> LeagueAvailablePlayers { get; set; } = new List<LeagueAvailablePlayer>();
        public ICollection<PlayerStatistic> PlayerStatistics { get; set; } = new List<PlayerStatistic>();

    }
    public record PlayerDto(string Id, string Name, int Role, string TeamId);
    public record PlayerIdAndNameDto(string Id, string Name);
    public record PlayerInLeagueDto(string Id, int leaguePlayerId, string Name, int Role, string TeamId, int Price);
}
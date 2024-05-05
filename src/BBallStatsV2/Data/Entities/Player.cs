using BBallStatsV2.Data.Entities;
using Microsoft.Extensions.Hosting;

namespace BBallStats.Data.Entities
{
	public enum PlayerRole
	{
		PointGuard = 0,
		ShootingGuard = 1,
		SmallForward = 2,
		PowerForward = 3,
		Center = 4
	}

	public class Player
	{
		public string Id { get; set; }
		public required string Name { get; set; }
		public PlayerRole Role { get; set; }
		public string CurrentTeamId { get; set; }
		public Team CurrentTeam { get; set; }
        public bool SkippedLastGame { get; set; }
        public ICollection<LeagueAvailablePlayer> LeagueAvailablePlayers { get; set; } = new List<LeagueAvailablePlayer>();
        public ICollection<PlayerStatistic> PlayerStatistics { get; set; } = new List<PlayerStatistic>();

    }
    public record PlayerDto(string Id, string Name, int Role, string TeamId);
    public record PlayerInLeagueDto(string Id, int leaguePlayerId, string Name, int Role, string TeamId, int Price);
}
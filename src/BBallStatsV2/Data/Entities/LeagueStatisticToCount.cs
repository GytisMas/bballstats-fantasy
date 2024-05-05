using System.Reflection.Metadata;

namespace BBallStatsV2.Data.Entities
{
    public class LeagueStatisticToCount
    {
        public int Id { get; set; }
        public double PointsPerStat { get; set; }
        public LeaguePlayerRole LeaguePlayerRole { get; set; } = null!;
        public int LeaguePlayerRoleId { get; set; }
        public int StatisticId { get; set; }
    }
}

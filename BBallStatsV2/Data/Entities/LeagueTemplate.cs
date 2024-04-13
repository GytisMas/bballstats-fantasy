using Microsoft.Extensions.Hosting;

namespace BBallStatsV2.Data.Entities
{
    public class LeagueTemplate
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public double? BenchMultiplier { get; set; }
        public IList<LeaguePlayerRole> Roles { get; set; } = new List<LeaguePlayerRole>();
        public IList<League> Leagues { get; set; } = new List<League>();
        public double TeamWinPoints { get; set; }
        public double TeamLosePoints { get; set; }
    }
}

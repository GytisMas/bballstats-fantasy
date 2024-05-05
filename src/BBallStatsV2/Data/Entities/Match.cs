using BBallStats.Data.Entities;

namespace BBallStatsV2.Data.Entities
{
    public class Match
    {
        public int Id { get; set; }
        public bool UsedInFantasy { get; set; }
        public int SeasonId { get; set; }
        public int GameId { get; set; }
        public Team HomeTeam { get; set; } = null!;
        public string HomeTeamId { get; set; } = null!;
        public Team AwayTeam { get; set; } = null!;
        public string AwayTeamId { get; set; } = null!;
        public DateTime MatchDate { get; set; }
    }
}

namespace BBallStatsV2.Data.Entities
{
    public class LeaguePlayerRole
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public LeagueTemplate LeagueTemplate { get; set; } = null!;
        public int LeagueTemplateId { get; set; }
        public int? RoleToReplaceId { get; set; }
        public LeaguePlayerRole? RoleToReplace { get; set; }
        public IList<LeagueStatisticToCount> Statistics { get; set; } = new List<LeagueStatisticToCount>();
    }
}

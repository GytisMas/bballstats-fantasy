using BBallStats.Data.Entities;

namespace BBallStatsV2.Data.Entities
{
    public class LeagueAvailablePlayer
    {
        public int Id { get; set; }
        public int Price { get; set; }
        public int LeagueId { get; set; }
        public League League { get; set; } = null!;
        public string PlayerId { get; set; } = null!;
        public Player Player { get; set; } = null!;
        public IList<ParticipantsRosterPlayer> UsedPlayers { get; } = new List<ParticipantsRosterPlayer>();
    }
}

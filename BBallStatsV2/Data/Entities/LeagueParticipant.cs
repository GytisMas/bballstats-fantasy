using BBallStats2.Auth.Model;

namespace BBallStatsV2.Data.Entities
{
    public class LeagueParticipant
    {
        public int Id { get; set; }
        public DateTime EntryDate { get; set; }
        public double Points { get; set; }
        public string UserId { get; set; } = null!;
        public ForumRestUser User { get; set; } = null!;
        public int LeagueId { get; set; }
        public League League { get; set; } = null!;
        public string TeamName { get; set; } = null!;
        public IList<ParticipantsRosterPlayer> Team { get; set; } = new List<ParticipantsRosterPlayer>();
    }
}

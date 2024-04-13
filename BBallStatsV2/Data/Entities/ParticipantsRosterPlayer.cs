using System.Reflection.Metadata;

namespace BBallStatsV2.Data.Entities
{
    public class ParticipantsRosterPlayer
    {
        public int Id { get; set; }
        public double Points { get; set; }
        public double PointsLastGame { get; set; }
        public int LeaguePlayerRoleId { get; set; }
        public LeaguePlayerRole LeaguePlayerRole { get; set; } = null!;
        public int LeagueParticipantId { get; set; }
        public LeagueParticipant LeagueParticipant { get; set; } = null!;
        public int LeagueAvailablePlayerId { get; set; }
        public LeagueAvailablePlayer LeagueAvailablePlayer { get; set; } = null!;
    }
}

using BBallStats2.Auth.Model;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Metadata;

namespace BBallStatsV2.Data.Entities
{
    public enum LeagueSortParameter
    {
        StartDateD = 0,
        StartDate = 1,
        EndDateD = 2,
        EndDate = 3,
        Name = 4,
        NameD = 5,
    }
    public class League
    {
        public int Id { get; set; }
        public string? Password { get; set; }
        public string Name { get; set; } = null!;
        public int EntryFee { get; set; }
        public int CollectedCurrency { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int LeagueTemplateId { get; set; }
        public LeagueTemplate LeagueTemplate { get; set; } = null!;
        public IList<LeagueParticipant> Participants { get; set; } = new List<LeagueParticipant>();
        public IList<LeaguePayment> Payments { get; set; } = new List<LeaguePayment>();
        public string LeagueHostId { get; set; } = null!;
        public ForumRestUser LeagueHost { get; set; } = null!;
        public bool IsOver { get; set; }
        public bool IsActive { 
            get {
                return !IsOver && DateTime.UtcNow >= StartDate && DateTime.UtcNow <= EndDate;
            } 
        }
        public bool HasStarted
        {
            get
            {
                return DateTime.UtcNow >= StartDate;
            }
        }
        public bool IsInPreparation { get { return DateTime.Now < StartDate; } }
        public IList<LeagueAvailablePlayer> LeagueAvailablePlayers { get; set; } = new List<LeagueAvailablePlayer>();
    }

    public record LeagueDto(int Id, int Entryfee, DateTime CreationDate, DateTime StartDate, DateTime EndDate, int LeagueTemplateId, string LeagueHostId);
    public record ListedLeagueDto(int Id, string Name, int EntryFee, bool IsActive, DateTime StartDate,
        DateTime EndDate, string LeagueTemplateName, string LeagueHostName, string[] AvailablePlayerTeamNames);

}

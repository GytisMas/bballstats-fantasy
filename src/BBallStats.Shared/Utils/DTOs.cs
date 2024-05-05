namespace BBallStats.Shared.Utils
{
    public class DTOs
    {
        public record CreateMatchDto(int gameCode, int seasonCode, string HomeTeamId, string AwayTeamId, DateTime MatchDate);
        public record MatchDto(int gameCode, int seasonCode, string HomeTeamId, string AwayTeamId, DateTime MatchDate);
        public record TeamDto(string Id, string Name);
        public record TeamNameDto(string Name);

    }
}

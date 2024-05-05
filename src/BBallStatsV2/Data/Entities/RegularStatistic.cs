namespace BBallStats.Data.Entities
{
	public class RegularStatistic : Statistic
    {
        public record CreateRegularStatisticDto(string Name, string? DisplayName, Visibility Status, double DefaultLeaguePointsPerStat, bool DefaultIsChecked);
        public record RegularStatisticDto(int Id, string Name, string? DisplayName, Visibility Status, double DefaultLeaguePointsPerStat, bool DefaultIsChecked);
    }
}
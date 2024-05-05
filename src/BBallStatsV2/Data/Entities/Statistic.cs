namespace BBallStats.Data.Entities
{
	public enum Visibility
	{
		Regular = 0,
		Main = 1
	}

	public class Statistic
    {
        public int Id { get; set; }
        public required string Name { get; set; }
		public string? DisplayName { get; set; }
		public required Visibility Status { get; set; }
		public double DefaultLeaguePointsPerStat { get; set; }
		public bool DefaultIsChecked { get; set; }
        public record StatisticDto(int Id, string Name, string DisplayName, int Status, bool DefaultIsChecked);
    }
}
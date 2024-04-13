namespace BBallStats.Data.Entities
{
    public class TeamStatistic
    {
        public int Id { get; set; }
        public required float Value { get; set; }
        public required float AttemptValue { get; set; }
        public required RegularStatistic Statistic { get; set; }
        public required int StatisticId { get; set; }
        public required Team Team { get; set; }
        public string TeamId { get; set; } = null!;

        public record TeamStatisticDto(int Id, float value, int StatType, int Team);

    }
}

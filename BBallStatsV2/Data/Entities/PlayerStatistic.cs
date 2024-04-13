namespace BBallStats.Data.Entities
{
    public class PlayerStatistic
    {
        public int Id { get; set; }
        public float Value { get; set; } = 0;
        public float AttemptValue { get; set; }
        public int GameCount { get; set; } = 0;
        public RegularStatistic Statistic { get; set; } = null!;
        public int StatisticId { get; set; }
        public Player Player { get; set; } = null!;
        public string PlayerId { get; set; } = null!;

        public record PlayerStatisticDto(int Id, float value, int StatType, string Player);

    }
}

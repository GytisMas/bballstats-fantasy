using BBallStats.Data.Entities;

namespace BBallStatsV2.Data.Entities
{
    public class CustomStatisticRegularStatistic
    {
        public int Id { get; set; }
        public int CustomStatisticId { get; set; }
        public int RegularStatisticId { get; set; }
    }
}

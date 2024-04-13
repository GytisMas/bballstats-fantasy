using BBallStats2.Auth.Model;
using System.ComponentModel.DataAnnotations;

namespace BBallStats.Data.Entities
{
	public class CustomStatistic : Statistic
	{
		public required string Formula { get; set; }
        public ForumRestUser User { get; set; } = null!;
        public required string UserId { get; set; }
        public IList<RegularStatistic> Statistics { get; set; } = new List<RegularStatistic>();

    }
    public record CustomStatisticDto(int Id, string Name, string Formula, Visibility Status, string AuthorId);
    public record CustomStatisticWithStatsDto(int Id, string Name, string Formula, Visibility Status, string AuthorId, int[] statIds);
    public record CreateCustomStatisticDto(string Name, string Formula, Visibility Status);
    public record UpdateCustomStatisticDto(string Name, string Formula, Visibility Status);
}
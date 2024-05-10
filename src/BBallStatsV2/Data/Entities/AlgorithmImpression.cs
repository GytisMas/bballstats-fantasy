using BBallStats2.Auth.Model;
using System.ComponentModel.DataAnnotations;

namespace BBallStats.Data.Entities
{
    public class AlgorithmImpression
    {
        public int Id { get; set; }
        public required bool Positive { get; set; }
        public CustomStatistic CustomStatistic { get; set; } = null!;
        public int CustomStatisticId { get; set; }

        public ForumRestUser User { get; set; } = null!;
        public required string UserId { get; set; }
    }
    public record AlgorithmImpressionShortDto(int Id, bool Positive, int Algorithm);
    public record AlgorithmImpressionDto(int Id, bool Positive, int Algorithm, string UserId);
    public record CreateAlgorithmImpressionDto(bool Positive, int Algorithm);
    public record UpdateAlgorithmImpressionDto(bool Positive);
}
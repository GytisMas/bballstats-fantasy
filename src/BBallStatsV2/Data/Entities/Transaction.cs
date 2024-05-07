using BBallStats2.Auth.Model;

namespace BBallStatsV2.Data.Entities
{
    public enum TransactionType
    {
        LeagueEntry = 0,
        LeaguePrizePayment = 1,
        Bonus = 2,
        LeagueCreation = 3,
        LeagueTemplateCreation = 4,
    }
    public class Transaction
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public TransactionType TransactionType { get; set; }
        public int Amount { get; set; }
        public string? SenderId { get; set; }
        public ForumRestUser? Sender { get; set; }
        public string? RecipientId { get; set; } = null!;
        public ForumRestUser? Recipient { get; set; } = null!;
    }
}

using Microsoft.EntityFrameworkCore;
using Mono.TextTemplating;

namespace BBallStatsV2.Data.Entities
{
    [PrimaryKey(nameof(LeagueId), nameof(Placing))]
    public class LeaguePayment
    {
        public int LeagueId { get; set; }
        public int Placing { get; set; }
        public int Amount { get; set; }
    }
}

using BBallStats.Data.Entities;
using BBallStats2.Auth.Model;
using BBallStatsV2.Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BBallStats.Data
{
    public class ForumDbContext : IdentityDbContext<ForumRestUser>
    {
        private readonly IConfiguration _configuration;
        public DbSet<Team> Teams { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<PlayerStatistic> PlayerStatistics { get; set; }
        public DbSet<TeamStatistic> TeamStatistics { get; set; }
        public DbSet<Statistic> Statistics { get; set; }
        public DbSet<RegularStatistic> RegularStatistics { get; set; }
        public DbSet<CustomStatistic> CustomStatistics { get; set; }
        public DbSet<CustomStatisticRegularStatistic> CustomStatisticRegularStatistic { get; set; }
        public DbSet<AlgorithmImpression> AlgorithmImpressions { get; set; }
        public DbSet<League> Leagues { get; set; }
        public DbSet<LeagueTemplate> LeagueTemplates { get; set; }
        public DbSet<LeaguePlayerRole> LeaguePlayerRoles { get; set; }
        public DbSet<LeagueAvailablePlayer> LeagueAvailablePlayers { get; set; }
        public DbSet<LeagueParticipant> LeagueParticipants { get; set; }
        public DbSet<LeagueStatisticToCount> LeagueStatisticsToCount { get; set; }
        public DbSet<ParticipantsRosterPlayer> ParticipantsRosterPlayers { get; set; }
        public DbSet<Match> Matches { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        public ForumDbContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<League>()
                .HasMany(e => e.Participants)
                .WithOne(e => e.League)
                .HasForeignKey(e => e.LeagueId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.Entity<League>()
                .HasOne(e => e.LeagueHost)
                .WithMany()
                .HasForeignKey(e => e.LeagueHostId);
            builder.Entity<League>()
                .HasOne(x => x.LeagueTemplate)
                .WithMany(x => x.Leagues)
                .HasForeignKey(e => e.LeagueTemplateId);
            builder.Entity<League>()
                .HasMany(e => e.LeagueAvailablePlayers)
                .WithOne(e => e.League)
                .HasForeignKey(e => e.LeagueId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<CustomStatistic>()
                .HasMany<AlgorithmImpression>()
                .WithOne(e => e.CustomStatistic)
                .HasForeignKey(e => e.CustomStatisticId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<CustomStatistic>()
                .HasMany(e => e.Statistics)
                .WithMany()
                .UsingEntity<CustomStatisticRegularStatistic>();

            builder.Entity<PlayerStatistic>()
                .HasOne(e => e.Player)
                .WithMany(e => e.PlayerStatistics)
                .HasForeignKey(e => e.PlayerId);

            builder.Entity<LeagueParticipant>()
                .HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(e => e.UserId);

            builder.Entity<LeagueStatisticToCount>()
                .HasOne<Statistic>()
                .WithMany()
                .HasForeignKey(e => e.StatisticId);

            builder.Entity<LeagueStatisticToCount>()
                .HasOne(x => x.LeaguePlayerRole)
                .WithMany(y => y.Statistics)
                .HasForeignKey(e => e.LeaguePlayerRoleId);

            builder.Entity<LeaguePlayerRole>()
                .HasOne(x => x.LeagueTemplate)
                .WithMany(y => y.Roles)
                .HasForeignKey(e => e.LeagueTemplateId);

            builder.Entity<Match>()
                .HasOne(x => x.HomeTeam)
                .WithMany()
                .HasForeignKey(x => x.HomeTeamId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Match>()
                .HasOne(x => x.AwayTeam)
                .WithMany()
                .HasForeignKey(x => x.AwayTeamId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Statistic>()
                .Property(b => b.DefaultLeaguePointsPerStat)
                .HasDefaultValue(1);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(_configuration.GetConnectionString("PostgreServerConnection"));
        }
    }
}

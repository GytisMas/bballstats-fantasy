﻿// <auto-generated />
using System;
using BBallStats.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace BBallStatsV2.Migrations
{
    [DbContext(typeof(ForumDbContext))]
    [Migration("20240320083650_RoleTemplate")]
    partial class RoleTemplate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("BBallStats.Data.Entities.AlgorithmImpression", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("CustomStatisticId")
                        .HasColumnType("int");

                    b.Property<bool>("Positive")
                        .HasColumnType("bit");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("CustomStatisticId");

                    b.HasIndex("UserId");

                    b.ToTable("AlgorithmImpressions");
                });

            modelBuilder.Entity("BBallStats.Data.Entities.Player", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("CurrentTeamId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Price")
                        .HasColumnType("int");

                    b.Property<int>("Role")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CurrentTeamId");

                    b.ToTable("Players");
                });

            modelBuilder.Entity("BBallStats.Data.Entities.PlayerStatistic", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<float>("AttemptValue")
                        .HasColumnType("real");

                    b.Property<int>("GameCount")
                        .HasColumnType("int");

                    b.Property<string>("PlayerId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("StatisticId")
                        .HasColumnType("int");

                    b.Property<float>("Value")
                        .HasColumnType("real");

                    b.HasKey("Id");

                    b.HasIndex("PlayerId");

                    b.HasIndex("StatisticId");

                    b.ToTable("PlayerStatistics");
                });

            modelBuilder.Entity("BBallStats.Data.Entities.Statistic", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasMaxLength(21)
                        .HasColumnType("nvarchar(21)");

                    b.Property<string>("DisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Statistics");

                    b.HasDiscriminator<string>("Discriminator").HasValue("Statistic");

                    b.UseTphMappingStrategy();
                });

            modelBuilder.Entity("BBallStats.Data.Entities.Team", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Teams");
                });

            modelBuilder.Entity("BBallStats.Data.Entities.TeamStatistic", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<float>("AttemptValue")
                        .HasColumnType("real");

                    b.Property<int>("StatisticId")
                        .HasColumnType("int");

                    b.Property<string>("TeamId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<float>("Value")
                        .HasColumnType("real");

                    b.HasKey("Id");

                    b.HasIndex("StatisticId");

                    b.HasIndex("TeamId");

                    b.ToTable("TeamStatistics");
                });

            modelBuilder.Entity("BBallStats2.Auth.Model.ForumRestUser", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Currency")
                        .HasColumnType("int");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<bool>("ForceRelogin")
                        .HasColumnType("bit");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers", (string)null);
                });

            modelBuilder.Entity("BBallStatsV2.Data.Entities.League", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("CollectedCurrency")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("EndDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("EntryFee")
                        .HasColumnType("int");

                    b.Property<string>("LeagueHostId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("LeagueTemplateId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("LeagueHostId");

                    b.HasIndex("LeagueTemplateId");

                    b.ToTable("Leagues");
                });

            modelBuilder.Entity("BBallStatsV2.Data.Entities.LeagueAvailablePlayer", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("LeagueId")
                        .HasColumnType("int");

                    b.Property<string>("PlayerId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("Price")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("LeagueId");

                    b.HasIndex("PlayerId");

                    b.ToTable("LeagueAvailablePlayers");
                });

            modelBuilder.Entity("BBallStatsV2.Data.Entities.LeagueParticipant", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("LeagueId")
                        .HasColumnType("int");

                    b.Property<double>("Points")
                        .HasColumnType("float");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("LeagueId");

                    b.HasIndex("UserId");

                    b.ToTable("LeagueParticipants");
                });

            modelBuilder.Entity("BBallStatsV2.Data.Entities.LeaguePlayerRole", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("LeagueTemplateId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("LeagueTemplateId");

                    b.ToTable("LeaguePlayerRoles");
                });

            modelBuilder.Entity("BBallStatsV2.Data.Entities.LeagueStatisticToCount", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("LeaguePlayerRoleId")
                        .HasColumnType("int");

                    b.Property<double>("PointsPerStat")
                        .HasColumnType("float");

                    b.Property<int>("StatisticId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("LeaguePlayerRoleId");

                    b.HasIndex("StatisticId");

                    b.ToTable("LeagueStatisticsToCount");
                });

            modelBuilder.Entity("BBallStatsV2.Data.Entities.LeagueTemplate", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("LeagueTemplates");
                });

            modelBuilder.Entity("BBallStatsV2.Data.Entities.ParticipantsRosterPlayer", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("LeagueAvailablePlayerId")
                        .HasColumnType("int");

                    b.Property<int>("LeagueParticipantId")
                        .HasColumnType("int");

                    b.Property<int>("LeaguePlayerRoleId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("LeagueAvailablePlayerId");

                    b.HasIndex("LeagueParticipantId");

                    b.HasIndex("LeaguePlayerRoleId");

                    b.ToTable("ParticipantsRosterPlayers");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("RoleId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens", (string)null);
                });

            modelBuilder.Entity("BBallStats.Data.Entities.CustomStatistic", b =>
                {
                    b.HasBaseType("BBallStats.Data.Entities.Statistic");

                    b.Property<string>("Formula")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasIndex("UserId");

                    b.HasDiscriminator().HasValue("CustomStatistic");
                });

            modelBuilder.Entity("BBallStats.Data.Entities.RegularStatistic", b =>
                {
                    b.HasBaseType("BBallStats.Data.Entities.Statistic");

                    b.Property<int?>("CustomStatisticId")
                        .HasColumnType("int");

                    b.HasIndex("CustomStatisticId");

                    b.HasDiscriminator().HasValue("RegularStatistic");
                });

            modelBuilder.Entity("BBallStats.Data.Entities.AlgorithmImpression", b =>
                {
                    b.HasOne("BBallStats.Data.Entities.CustomStatistic", "CustomStatistic")
                        .WithMany()
                        .HasForeignKey("CustomStatisticId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("BBallStats2.Auth.Model.ForumRestUser", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("CustomStatistic");

                    b.Navigation("User");
                });

            modelBuilder.Entity("BBallStats.Data.Entities.Player", b =>
                {
                    b.HasOne("BBallStats.Data.Entities.Team", "CurrentTeam")
                        .WithMany()
                        .HasForeignKey("CurrentTeamId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("CurrentTeam");
                });

            modelBuilder.Entity("BBallStats.Data.Entities.PlayerStatistic", b =>
                {
                    b.HasOne("BBallStats.Data.Entities.Player", "Player")
                        .WithMany("PlayerStatistics")
                        .HasForeignKey("PlayerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BBallStats.Data.Entities.RegularStatistic", "Statistic")
                        .WithMany()
                        .HasForeignKey("StatisticId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Player");

                    b.Navigation("Statistic");
                });

            modelBuilder.Entity("BBallStats.Data.Entities.TeamStatistic", b =>
                {
                    b.HasOne("BBallStats.Data.Entities.RegularStatistic", "Statistic")
                        .WithMany()
                        .HasForeignKey("StatisticId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BBallStats.Data.Entities.Team", "Team")
                        .WithMany()
                        .HasForeignKey("TeamId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Statistic");

                    b.Navigation("Team");
                });

            modelBuilder.Entity("BBallStatsV2.Data.Entities.League", b =>
                {
                    b.HasOne("BBallStats2.Auth.Model.ForumRestUser", "LeagueHost")
                        .WithMany()
                        .HasForeignKey("LeagueHostId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BBallStatsV2.Data.Entities.LeagueTemplate", "LeagueTemplate")
                        .WithMany("Leagues")
                        .HasForeignKey("LeagueTemplateId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("LeagueHost");

                    b.Navigation("LeagueTemplate");
                });

            modelBuilder.Entity("BBallStatsV2.Data.Entities.LeagueAvailablePlayer", b =>
                {
                    b.HasOne("BBallStatsV2.Data.Entities.League", "League")
                        .WithMany("LeagueAvailablePlayers")
                        .HasForeignKey("LeagueId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("BBallStats.Data.Entities.Player", null)
                        .WithMany("LeagueAvailablePlayers")
                        .HasForeignKey("PlayerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("League");
                });

            modelBuilder.Entity("BBallStatsV2.Data.Entities.LeagueParticipant", b =>
                {
                    b.HasOne("BBallStatsV2.Data.Entities.League", "League")
                        .WithMany("Participants")
                        .HasForeignKey("LeagueId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("BBallStats2.Auth.Model.ForumRestUser", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("League");

                    b.Navigation("User");
                });

            modelBuilder.Entity("BBallStatsV2.Data.Entities.LeaguePlayerRole", b =>
                {
                    b.HasOne("BBallStatsV2.Data.Entities.LeagueTemplate", "LeagueTemplate")
                        .WithMany("Roles")
                        .HasForeignKey("LeagueTemplateId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("LeagueTemplate");
                });

            modelBuilder.Entity("BBallStatsV2.Data.Entities.LeagueStatisticToCount", b =>
                {
                    b.HasOne("BBallStatsV2.Data.Entities.LeaguePlayerRole", "LeaguePlayerRole")
                        .WithMany("Statistics")
                        .HasForeignKey("LeaguePlayerRoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BBallStats.Data.Entities.Statistic", null)
                        .WithMany()
                        .HasForeignKey("StatisticId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("LeaguePlayerRole");
                });

            modelBuilder.Entity("BBallStatsV2.Data.Entities.ParticipantsRosterPlayer", b =>
                {
                    b.HasOne("BBallStatsV2.Data.Entities.LeagueAvailablePlayer", "LeagueAvailablePlayer")
                        .WithMany("UsedPlayers")
                        .HasForeignKey("LeagueAvailablePlayerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BBallStatsV2.Data.Entities.LeagueParticipant", "LeagueParticipant")
                        .WithMany("Team")
                        .HasForeignKey("LeagueParticipantId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BBallStatsV2.Data.Entities.LeaguePlayerRole", "LeaguePlayerRole")
                        .WithMany()
                        .HasForeignKey("LeaguePlayerRoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("LeagueAvailablePlayer");

                    b.Navigation("LeagueParticipant");

                    b.Navigation("LeaguePlayerRole");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("BBallStats2.Auth.Model.ForumRestUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("BBallStats2.Auth.Model.ForumRestUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BBallStats2.Auth.Model.ForumRestUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("BBallStats2.Auth.Model.ForumRestUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("BBallStats.Data.Entities.CustomStatistic", b =>
                {
                    b.HasOne("BBallStats2.Auth.Model.ForumRestUser", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("BBallStats.Data.Entities.RegularStatistic", b =>
                {
                    b.HasOne("BBallStats.Data.Entities.CustomStatistic", null)
                        .WithMany("Statistics")
                        .HasForeignKey("CustomStatisticId");
                });

            modelBuilder.Entity("BBallStats.Data.Entities.Player", b =>
                {
                    b.Navigation("LeagueAvailablePlayers");

                    b.Navigation("PlayerStatistics");
                });

            modelBuilder.Entity("BBallStatsV2.Data.Entities.League", b =>
                {
                    b.Navigation("LeagueAvailablePlayers");

                    b.Navigation("Participants");
                });

            modelBuilder.Entity("BBallStatsV2.Data.Entities.LeagueAvailablePlayer", b =>
                {
                    b.Navigation("UsedPlayers");
                });

            modelBuilder.Entity("BBallStatsV2.Data.Entities.LeagueParticipant", b =>
                {
                    b.Navigation("Team");
                });

            modelBuilder.Entity("BBallStatsV2.Data.Entities.LeaguePlayerRole", b =>
                {
                    b.Navigation("Statistics");
                });

            modelBuilder.Entity("BBallStatsV2.Data.Entities.LeagueTemplate", b =>
                {
                    b.Navigation("Leagues");

                    b.Navigation("Roles");
                });

            modelBuilder.Entity("BBallStats.Data.Entities.CustomStatistic", b =>
                {
                    b.Navigation("Statistics");
                });
#pragma warning restore 612, 618
        }
    }
}

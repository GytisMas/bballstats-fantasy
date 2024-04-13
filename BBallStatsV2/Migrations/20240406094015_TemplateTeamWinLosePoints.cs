using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BBallStatsV2.Migrations
{
    /// <inheritdoc />
    public partial class TemplateTeamWinLosePoints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Points",
                table: "ParticipantsRosterPlayers",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "PointsLastGame",
                table: "ParticipantsRosterPlayers",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "TeamLosePoints",
                table: "LeagueTemplates",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "TeamWinPoints",
                table: "LeagueTemplates",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Points",
                table: "ParticipantsRosterPlayers");

            migrationBuilder.DropColumn(
                name: "PointsLastGame",
                table: "ParticipantsRosterPlayers");

            migrationBuilder.DropColumn(
                name: "TeamLosePoints",
                table: "LeagueTemplates");

            migrationBuilder.DropColumn(
                name: "TeamWinPoints",
                table: "LeagueTemplates");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BBallStatsV2.Migrations
{
    /// <inheritdoc />
    public partial class StartFiveMode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "SkippedLastGame",
                table: "Players",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "GameId",
                table: "Matches",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SeasonId",
                table: "Matches",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "BenchMultiplier",
                table: "LeagueTemplates",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RoleToReplaceId",
                table: "LeaguePlayerRoles",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "StartFive",
                table: "LeaguePlayerRoles",
                type: "bit",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SkippedLastGame",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "GameId",
                table: "Matches");

            migrationBuilder.DropColumn(
                name: "SeasonId",
                table: "Matches");

            migrationBuilder.DropColumn(
                name: "BenchMultiplier",
                table: "LeagueTemplates");

            migrationBuilder.DropColumn(
                name: "RoleToReplaceId",
                table: "LeaguePlayerRoles");

            migrationBuilder.DropColumn(
                name: "StartFive",
                table: "LeaguePlayerRoles");
        }
    }
}

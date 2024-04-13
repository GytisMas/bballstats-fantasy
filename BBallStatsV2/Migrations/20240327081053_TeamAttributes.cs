using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BBallStatsV2.Migrations
{
    /// <inheritdoc />
    public partial class TeamAttributes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LogoLink",
                table: "Teams",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TeamName",
                table: "LeagueParticipants",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LogoLink",
                table: "Teams");

            migrationBuilder.DropColumn(
                name: "TeamName",
                table: "LeagueParticipants");
        }
    }
}

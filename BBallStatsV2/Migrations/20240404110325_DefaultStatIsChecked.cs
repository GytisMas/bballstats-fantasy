using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BBallStatsV2.Migrations
{
    /// <inheritdoc />
    public partial class DefaultStatIsChecked : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StartFive",
                table: "LeaguePlayerRoles");

            migrationBuilder.AddColumn<bool>(
                name: "DefaultIsChecked",
                table: "Statistics",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DefaultIsChecked",
                table: "Statistics");

            migrationBuilder.AddColumn<bool>(
                name: "StartFive",
                table: "LeaguePlayerRoles",
                type: "bit",
                nullable: true);
        }
    }
}

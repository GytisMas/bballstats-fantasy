using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BBallStatsV2.Migrations
{
    /// <inheritdoc />
    public partial class MatchUsedInFantasy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "UsedInFantasy",
                table: "Matches",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UsedInFantasy",
                table: "Matches");
        }
    }
}

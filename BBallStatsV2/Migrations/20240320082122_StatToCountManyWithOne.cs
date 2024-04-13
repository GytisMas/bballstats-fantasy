using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BBallStatsV2.Migrations
{
    /// <inheritdoc />
    public partial class StatToCountManyWithOne : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_LeagueStatisticsToCount_StatisticId",
                table: "LeagueStatisticsToCount");

            migrationBuilder.CreateIndex(
                name: "IX_LeagueStatisticsToCount_StatisticId",
                table: "LeagueStatisticsToCount",
                column: "StatisticId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_LeagueStatisticsToCount_StatisticId",
                table: "LeagueStatisticsToCount");

            migrationBuilder.CreateIndex(
                name: "IX_LeagueStatisticsToCount_StatisticId",
                table: "LeagueStatisticsToCount",
                column: "StatisticId",
                unique: true);
        }
    }
}

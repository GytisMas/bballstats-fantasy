using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BBallStatsV2.Migrations
{
    /// <inheritdoc />
    public partial class CustomStatisticManyToManyFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomStatisticRegularStatistic_Statistics_CustomStatisticId",
                table: "CustomStatisticRegularStatistic");

            migrationBuilder.RenameColumn(
                name: "CustomStatisticId",
                table: "CustomStatisticRegularStatistic",
                newName: "CustomStatisticsId");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomStatisticRegularStatistic_Statistics_CustomStatisticsId",
                table: "CustomStatisticRegularStatistic",
                column: "CustomStatisticsId",
                principalTable: "Statistics",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomStatisticRegularStatistic_Statistics_CustomStatisticsId",
                table: "CustomStatisticRegularStatistic");

            migrationBuilder.RenameColumn(
                name: "CustomStatisticsId",
                table: "CustomStatisticRegularStatistic",
                newName: "CustomStatisticId");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomStatisticRegularStatistic_Statistics_CustomStatisticId",
                table: "CustomStatisticRegularStatistic",
                column: "CustomStatisticId",
                principalTable: "Statistics",
                principalColumn: "Id");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BBallStatsV2.Migrations
{
    /// <inheritdoc />
    public partial class CustomStatisticManyToManyJoinTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomStatisticRegularStatistic_Statistics_StatisticsId",
                table: "CustomStatisticRegularStatistic");

            migrationBuilder.RenameColumn(
                name: "StatisticsId",
                table: "CustomStatisticRegularStatistic",
                newName: "RegularStatisticId");

            migrationBuilder.RenameIndex(
                name: "IX_CustomStatisticRegularStatistic_StatisticsId",
                table: "CustomStatisticRegularStatistic",
                newName: "IX_CustomStatisticRegularStatistic_RegularStatisticId");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomStatisticRegularStatistic_Statistics_RegularStatisticId",
                table: "CustomStatisticRegularStatistic",
                column: "RegularStatisticId",
                principalTable: "Statistics",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomStatisticRegularStatistic_Statistics_RegularStatisticId",
                table: "CustomStatisticRegularStatistic");

            migrationBuilder.RenameColumn(
                name: "RegularStatisticId",
                table: "CustomStatisticRegularStatistic",
                newName: "StatisticsId");

            migrationBuilder.RenameIndex(
                name: "IX_CustomStatisticRegularStatistic_RegularStatisticId",
                table: "CustomStatisticRegularStatistic",
                newName: "IX_CustomStatisticRegularStatistic_StatisticsId");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomStatisticRegularStatistic_Statistics_StatisticsId",
                table: "CustomStatisticRegularStatistic",
                column: "StatisticsId",
                principalTable: "Statistics",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

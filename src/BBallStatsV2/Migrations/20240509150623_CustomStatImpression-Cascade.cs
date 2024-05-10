using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BBallStatsV2.Migrations
{
    /// <inheritdoc />
    public partial class CustomStatImpressionCascade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AlgorithmImpressions_Statistics_CustomStatisticId",
                table: "AlgorithmImpressions");

            migrationBuilder.AddForeignKey(
                name: "FK_AlgorithmImpressions_Statistics_CustomStatisticId",
                table: "AlgorithmImpressions",
                column: "CustomStatisticId",
                principalTable: "Statistics",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AlgorithmImpressions_Statistics_CustomStatisticId",
                table: "AlgorithmImpressions");

            migrationBuilder.AddForeignKey(
                name: "FK_AlgorithmImpressions_Statistics_CustomStatisticId",
                table: "AlgorithmImpressions",
                column: "CustomStatisticId",
                principalTable: "Statistics",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

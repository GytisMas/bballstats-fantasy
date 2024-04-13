using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BBallStatsV2.Migrations
{
    /// <inheritdoc />
    public partial class CustomStatisticManyToManyId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomStatisticRegularStatistic_Statistics_CustomStatisticsId",
                table: "CustomStatisticRegularStatistic");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CustomStatisticRegularStatistic",
                table: "CustomStatisticRegularStatistic");

            migrationBuilder.RenameColumn(
                name: "CustomStatisticsId",
                table: "CustomStatisticRegularStatistic",
                newName: "CustomStatisticId");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "CustomStatisticRegularStatistic",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CustomStatisticRegularStatistic",
                table: "CustomStatisticRegularStatistic",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_CustomStatisticRegularStatistic_CustomStatisticId",
                table: "CustomStatisticRegularStatistic",
                column: "CustomStatisticId");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomStatisticRegularStatistic_Statistics_CustomStatisticId",
                table: "CustomStatisticRegularStatistic",
                column: "CustomStatisticId",
                principalTable: "Statistics",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomStatisticRegularStatistic_Statistics_CustomStatisticId",
                table: "CustomStatisticRegularStatistic");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CustomStatisticRegularStatistic",
                table: "CustomStatisticRegularStatistic");

            migrationBuilder.DropIndex(
                name: "IX_CustomStatisticRegularStatistic_CustomStatisticId",
                table: "CustomStatisticRegularStatistic");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "CustomStatisticRegularStatistic");

            migrationBuilder.RenameColumn(
                name: "CustomStatisticId",
                table: "CustomStatisticRegularStatistic",
                newName: "CustomStatisticsId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CustomStatisticRegularStatistic",
                table: "CustomStatisticRegularStatistic",
                columns: new[] { "CustomStatisticsId", "StatisticsId" });

            migrationBuilder.AddForeignKey(
                name: "FK_CustomStatisticRegularStatistic_Statistics_CustomStatisticsId",
                table: "CustomStatisticRegularStatistic",
                column: "CustomStatisticsId",
                principalTable: "Statistics",
                principalColumn: "Id");
        }
    }
}

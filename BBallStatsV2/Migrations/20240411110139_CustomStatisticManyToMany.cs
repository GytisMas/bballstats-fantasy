using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BBallStatsV2.Migrations
{
    /// <inheritdoc />
    public partial class CustomStatisticManyToMany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Statistics_Statistics_CustomStatisticId",
                table: "Statistics");

            migrationBuilder.DropIndex(
                name: "IX_Statistics_CustomStatisticId",
                table: "Statistics");

            migrationBuilder.DropColumn(
                name: "CustomStatisticId",
                table: "Statistics");

            migrationBuilder.CreateTable(
                name: "CustomStatisticRegularStatistic",
                columns: table => new
                {
                    CustomStatisticId = table.Column<int>(type: "int", nullable: false),
                    StatisticsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomStatisticRegularStatistic", x => new { x.CustomStatisticId, x.StatisticsId });
                    table.ForeignKey(
                        name: "FK_CustomStatisticRegularStatistic_Statistics_CustomStatisticId",
                        column: x => x.CustomStatisticId,
                        principalTable: "Statistics",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CustomStatisticRegularStatistic_Statistics_StatisticsId",
                        column: x => x.StatisticsId,
                        principalTable: "Statistics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomStatisticRegularStatistic_StatisticsId",
                table: "CustomStatisticRegularStatistic",
                column: "StatisticsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomStatisticRegularStatistic");

            migrationBuilder.AddColumn<int>(
                name: "CustomStatisticId",
                table: "Statistics",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Statistics_CustomStatisticId",
                table: "Statistics",
                column: "CustomStatisticId");

            migrationBuilder.AddForeignKey(
                name: "FK_Statistics_Statistics_CustomStatisticId",
                table: "Statistics",
                column: "CustomStatisticId",
                principalTable: "Statistics",
                principalColumn: "Id");
        }
    }
}

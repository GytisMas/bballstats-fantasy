using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BBallStatsV2.Migrations
{
    /// <inheritdoc />
    public partial class StatisticPointsToDouble : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "DefaultLeaguePointsPerStat",
                table: "Statistics",
                type: "float",
                nullable: false,
                defaultValue: 1.0,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 1);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "DefaultLeaguePointsPerStat",
                table: "Statistics",
                type: "int",
                nullable: false,
                defaultValue: 1,
                oldClrType: typeof(double),
                oldType: "float",
                oldDefaultValue: 1.0);
        }
    }
}

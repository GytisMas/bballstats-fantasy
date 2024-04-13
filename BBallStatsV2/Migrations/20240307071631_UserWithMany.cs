using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BBallStatsV2.Migrations
{
    /// <inheritdoc />
    public partial class UserWithMany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Players_Teams_CurrentTeamId",
                table: "Players");

            migrationBuilder.DropIndex(
                name: "IX_Leagues_LeagueHostId",
                table: "Leagues");

            migrationBuilder.DropIndex(
                name: "IX_LeagueParticipants_UserId",
                table: "LeagueParticipants");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentTeamId",
                table: "Players",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Leagues_LeagueHostId",
                table: "Leagues",
                column: "LeagueHostId");

            migrationBuilder.CreateIndex(
                name: "IX_LeagueParticipants_UserId",
                table: "LeagueParticipants",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Players_Teams_CurrentTeamId",
                table: "Players",
                column: "CurrentTeamId",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Players_Teams_CurrentTeamId",
                table: "Players");

            migrationBuilder.DropIndex(
                name: "IX_Leagues_LeagueHostId",
                table: "Leagues");

            migrationBuilder.DropIndex(
                name: "IX_LeagueParticipants_UserId",
                table: "LeagueParticipants");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentTeamId",
                table: "Players",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateIndex(
                name: "IX_Leagues_LeagueHostId",
                table: "Leagues",
                column: "LeagueHostId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LeagueParticipants_UserId",
                table: "LeagueParticipants",
                column: "UserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Players_Teams_CurrentTeamId",
                table: "Players",
                column: "CurrentTeamId",
                principalTable: "Teams",
                principalColumn: "Id");
        }
    }
}

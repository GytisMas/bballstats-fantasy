using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BBallStatsV2.Migrations
{
    /// <inheritdoc />
    public partial class StatToCountRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LeaguePlayerRole_LeagueTemplates_LeagueTemplateId",
                table: "LeaguePlayerRole");

            migrationBuilder.DropForeignKey(
                name: "FK_LeagueStatisticsToCount_LeaguePlayerRole_LeaguePlayerRoleId",
                table: "LeagueStatisticsToCount");

            migrationBuilder.DropForeignKey(
                name: "FK_ParticipantsRosterPlayers_LeaguePlayerRole_LeaguePlayerRoleId",
                table: "ParticipantsRosterPlayers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LeaguePlayerRole",
                table: "LeaguePlayerRole");

            migrationBuilder.RenameTable(
                name: "LeaguePlayerRole",
                newName: "LeaguePlayerRoles");

            migrationBuilder.RenameIndex(
                name: "IX_LeaguePlayerRole_LeagueTemplateId",
                table: "LeaguePlayerRoles",
                newName: "IX_LeaguePlayerRoles_LeagueTemplateId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LeaguePlayerRoles",
                table: "LeaguePlayerRoles",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LeaguePlayerRoles_LeagueTemplates_LeagueTemplateId",
                table: "LeaguePlayerRoles",
                column: "LeagueTemplateId",
                principalTable: "LeagueTemplates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LeagueStatisticsToCount_LeaguePlayerRoles_LeaguePlayerRoleId",
                table: "LeagueStatisticsToCount",
                column: "LeaguePlayerRoleId",
                principalTable: "LeaguePlayerRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ParticipantsRosterPlayers_LeaguePlayerRoles_LeaguePlayerRoleId",
                table: "ParticipantsRosterPlayers",
                column: "LeaguePlayerRoleId",
                principalTable: "LeaguePlayerRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LeaguePlayerRoles_LeagueTemplates_LeagueTemplateId",
                table: "LeaguePlayerRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_LeagueStatisticsToCount_LeaguePlayerRoles_LeaguePlayerRoleId",
                table: "LeagueStatisticsToCount");

            migrationBuilder.DropForeignKey(
                name: "FK_ParticipantsRosterPlayers_LeaguePlayerRoles_LeaguePlayerRoleId",
                table: "ParticipantsRosterPlayers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LeaguePlayerRoles",
                table: "LeaguePlayerRoles");

            migrationBuilder.RenameTable(
                name: "LeaguePlayerRoles",
                newName: "LeaguePlayerRole");

            migrationBuilder.RenameIndex(
                name: "IX_LeaguePlayerRoles_LeagueTemplateId",
                table: "LeaguePlayerRole",
                newName: "IX_LeaguePlayerRole_LeagueTemplateId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LeaguePlayerRole",
                table: "LeaguePlayerRole",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LeaguePlayerRole_LeagueTemplates_LeagueTemplateId",
                table: "LeaguePlayerRole",
                column: "LeagueTemplateId",
                principalTable: "LeagueTemplates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LeagueStatisticsToCount_LeaguePlayerRole_LeaguePlayerRoleId",
                table: "LeagueStatisticsToCount",
                column: "LeaguePlayerRoleId",
                principalTable: "LeaguePlayerRole",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ParticipantsRosterPlayers_LeaguePlayerRole_LeaguePlayerRoleId",
                table: "ParticipantsRosterPlayers",
                column: "LeaguePlayerRoleId",
                principalTable: "LeaguePlayerRole",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BBallStatsV2.Migrations
{
    /// <inheritdoc />
    public partial class RoleToReplace : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_LeaguePlayerRoles_RoleToReplaceId",
                table: "LeaguePlayerRoles",
                column: "RoleToReplaceId");

            migrationBuilder.AddForeignKey(
                name: "FK_LeaguePlayerRoles_LeaguePlayerRoles_RoleToReplaceId",
                table: "LeaguePlayerRoles",
                column: "RoleToReplaceId",
                principalTable: "LeaguePlayerRoles",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LeaguePlayerRoles_LeaguePlayerRoles_RoleToReplaceId",
                table: "LeaguePlayerRoles");

            migrationBuilder.DropIndex(
                name: "IX_LeaguePlayerRoles_RoleToReplaceId",
                table: "LeaguePlayerRoles");
        }
    }
}

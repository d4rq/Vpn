using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vpn.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class Add_Peer_Key : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Config_Users_UserId",
                table: "Config");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Config",
                table: "Config");

            migrationBuilder.RenameTable(
                name: "Config",
                newName: "Configs");

            migrationBuilder.RenameIndex(
                name: "IX_Config_UserId",
                table: "Configs",
                newName: "IX_Configs_UserId");

            migrationBuilder.AddColumn<string>(
                name: "ServerKey",
                table: "Configs",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Configs",
                table: "Configs",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Configs_Users_UserId",
                table: "Configs",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Configs_Users_UserId",
                table: "Configs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Configs",
                table: "Configs");

            migrationBuilder.DropColumn(
                name: "ServerKey",
                table: "Configs");

            migrationBuilder.RenameTable(
                name: "Configs",
                newName: "Config");

            migrationBuilder.RenameIndex(
                name: "IX_Configs_UserId",
                table: "Config",
                newName: "IX_Config_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Config",
                table: "Config",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Config_Users_UserId",
                table: "Config",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

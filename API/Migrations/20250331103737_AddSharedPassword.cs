using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    public partial class AddSharedPassword : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SharedPasswords",
                columns: table => new
                {
                    PasswordId = table.Column<string>(type: "text", nullable: false),
                    HashedPassword = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SharedPasswords", x => x.PasswordId);
                });

            migrationBuilder.AddColumn<string>(
                name: "SharedPasswordId",
                table: "Devices",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Devices_SharedPasswordId",
                table: "Devices",
                column: "SharedPasswordId");

            migrationBuilder.AddForeignKey(
                name: "FK_Devices_SharedPasswords_SharedPasswordId",
                table: "Devices",
                column: "SharedPasswordId",
                principalTable: "SharedPasswords",
                principalColumn: "PasswordId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Devices_SharedPasswords_SharedPasswordId",
                table: "Devices");

            migrationBuilder.DropTable(
                name: "SharedPasswords");

            migrationBuilder.DropColumn(
                name: "SharedPasswordId",
                table: "Devices");
        }
    }
}
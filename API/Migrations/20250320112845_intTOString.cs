using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class intTOString : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_User_Devices_Devices_DeviceId",
                table: "User_Devices");

            migrationBuilder.DropIndex(
                name: "IX_User_Devices_DeviceId",
                table: "User_Devices");

            migrationBuilder.AlterColumn<string>(
                name: "DeviceId",
                table: "User_Devices",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<int>(
                name: "DeviceId1",
                table: "User_Devices",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "DeviceId",
                table: "Devices",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.CreateIndex(
                name: "IX_User_Devices_DeviceId1",
                table: "User_Devices",
                column: "DeviceId1");

            migrationBuilder.AddForeignKey(
                name: "FK_User_Devices_Devices_DeviceId1",
                table: "User_Devices",
                column: "DeviceId1",
                principalTable: "Devices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_User_Devices_Devices_DeviceId1",
                table: "User_Devices");

            migrationBuilder.DropIndex(
                name: "IX_User_Devices_DeviceId1",
                table: "User_Devices");

            migrationBuilder.DropColumn(
                name: "DeviceId1",
                table: "User_Devices");

            migrationBuilder.AlterColumn<int>(
                name: "DeviceId",
                table: "User_Devices",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<int>(
                name: "DeviceId",
                table: "Devices",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.CreateIndex(
                name: "IX_User_Devices_DeviceId",
                table: "User_Devices",
                column: "DeviceId");

            migrationBuilder.AddForeignKey(
                name: "FK_User_Devices_Devices_DeviceId",
                table: "User_Devices",
                column: "DeviceId",
                principalTable: "Devices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

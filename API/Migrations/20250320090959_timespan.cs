using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class timespan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<TimeSpan>(
                name: "TriggeredTime",
                table: "Logs",
                type: "interval",
                nullable: true,
                oldClrType: typeof(TimeOnly),
                oldType: "time without time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "DisarmedTime",
                table: "Logs",
                type: "interval",
                nullable: false,
                oldClrType: typeof(TimeOnly),
                oldType: "time without time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Date",
                table: "Logs",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "ArmedTime",
                table: "Logs",
                type: "interval",
                nullable: false,
                oldClrType: typeof(TimeOnly),
                oldType: "time without time zone");

            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "Devices",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Password",
                table: "Devices");

            migrationBuilder.AlterColumn<TimeOnly>(
                name: "TriggeredTime",
                table: "Logs",
                type: "time without time zone",
                nullable: true,
                oldClrType: typeof(TimeSpan),
                oldType: "interval",
                oldNullable: true);

            migrationBuilder.AlterColumn<TimeOnly>(
                name: "DisarmedTime",
                table: "Logs",
                type: "time without time zone",
                nullable: false,
                oldClrType: typeof(TimeSpan),
                oldType: "interval");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "Date",
                table: "Logs",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<TimeOnly>(
                name: "ArmedTime",
                table: "Logs",
                type: "time without time zone",
                nullable: false,
                oldClrType: typeof(TimeSpan),
                oldType: "interval");
        }
    }
}

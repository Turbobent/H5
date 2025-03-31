using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class newnewtime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<TimeOnly>(
                name: "TriggeredTime",
                table: "Logs",
                type: "time without time zone",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<TimeOnly>(
                name: "DisarmedTime",
                table: "Logs",
                type: "time without time zone",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0),
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<TimeOnly>(
                name: "ArmedTime",
                table: "Logs",
                type: "time without time zone",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "TriggeredTime",
                table: "Logs",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(TimeOnly),
                oldType: "time without time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "DisarmedTime",
                table: "Logs",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(TimeOnly),
                oldType: "time without time zone");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "ArmedTime",
                table: "Logs",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(TimeOnly),
                oldType: "time without time zone");
        }
    }
}

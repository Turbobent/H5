//using Microsoft.EntityFrameworkCore.Migrations;
//using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

//#nullable disable

//namespace API.Migrations
//{
//    public partial class newtime : Migration
//    {
//        protected override void Up(MigrationBuilder migrationBuilder)
//        {
//            // Convert timestamp columns
//            migrationBuilder.AlterColumn<DateTime>(
//                name: "DisarmedTime",
//                table: "Logs",
//                type: "timestamp with time zone",
//                nullable: false,
//                oldClrType: typeof(DateTime),
//                oldType: "timestamp without time zone");

//            migrationBuilder.AlterColumn<DateTime>(
//                name: "ArmedTime",
//                table: "Logs",
//                type: "timestamp with time zone",
//                nullable: false,
//                oldClrType: typeof(DateTime),
//                oldType: "timestamp without time zone");

//            migrationBuilder.AlterColumn<DateTime>(
//                name: "TriggeredTime",
//                table: "Logs",
//                type: "timestamp with time zone",
//                nullable: true,
//                oldClrType: typeof(DateTime),
//                oldType: "timestamp without time zone",
//                oldNullable: true);

//            // Remove DeviceId1 column
//            migrationBuilder.DropColumn(
//                name: "DeviceId1",
//                table: "User_Devices");
//        }

//        protected override void Down(MigrationBuilder migrationBuilder)
//        {
//            // Revert timestamp columns
//            migrationBuilder.AlterColumn<DateTime>(
//                name: "DisarmedTime",
//                table: "Logs",
//                type: "timestamp without time zone",
//                nullable: false,
//                oldClrType: typeof(DateTime),
//                oldType: "timestamp with time zone");

//            migrationBuilder.AlterColumn<DateTime>(
//                name: "ArmedTime",
//                table: "Logs",
//                type: "timestamp without time zone",
//                nullable: false,
//                oldClrType: typeof(DateTime),
//                oldType: "timestamp with time zone");

//            migrationBuilder.AlterColumn<DateTime>(
//                name: "TriggeredTime",
//                table: "Logs",
//                type: "timestamp without time zone",
//                nullable: true,
//                oldClrType: typeof(DateTime),
//                oldType: "timestamp with time zone",
//                oldNullable: true);

//            // Re-add DeviceId1 column
//            migrationBuilder.AddColumn<int>(
//                name: "DeviceId1",
//                table: "User_Devices",
//                type: "integer",
//                nullable: false,
//                defaultValue: 0);
//        }
//    }
//}
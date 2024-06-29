using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Data.Migrations
{
    /// <inheritdoc />
    public partial class Flights : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CVRefId",
                table: "CandidateFlights",
                newName: "CvRefId");

            migrationBuilder.RenameIndex(
                name: "IX_CandidateFlights_CVRefId",
                table: "CandidateFlights",
                newName: "IX_CandidateFlights_CvRefId");

            migrationBuilder.AddColumn<int>(
                name: "CandidateFlightId",
                table: "DepItems",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateOfFlight",
                table: "CandidateFlights",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "DepItemId",
                table: "CandidateFlights",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<DateTime>(
                name: "ETD_Via",
                table: "CandidateFlightDetails",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ETA_Via",
                table: "CandidateFlightDetails",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "TEXT");

            migrationBuilder.CreateTable(
                name: "FlightDatas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    flightNo = table.Column<string>(type: "TEXT", nullable: true),
                    AirportOfBoarding = table.Column<string>(type: "TEXT", nullable: true),
                    AirportOfDestination = table.Column<string>(type: "TEXT", nullable: true),
                    AirportVia = table.Column<string>(type: "TEXT", nullable: true),
                    FlightNoVia = table.Column<string>(type: "TEXT", nullable: true),
                    ETD_Boarding = table.Column<TimeOnly>(type: "TEXT", nullable: false),
                    ETA_Destination = table.Column<TimeOnly>(type: "TEXT", nullable: false),
                    ETA_Via = table.Column<TimeOnly>(type: "TEXT", nullable: false),
                    ETD_Via = table.Column<TimeOnly>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FlightDatas", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DepItems_CandidateFlightId",
                table: "DepItems",
                column: "CandidateFlightId");

            migrationBuilder.AddForeignKey(
                name: "FK_DepItems_CandidateFlights_CandidateFlightId",
                table: "DepItems",
                column: "CandidateFlightId",
                principalTable: "CandidateFlights",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DepItems_CandidateFlights_CandidateFlightId",
                table: "DepItems");

            migrationBuilder.DropTable(
                name: "FlightDatas");

            migrationBuilder.DropIndex(
                name: "IX_DepItems_CandidateFlightId",
                table: "DepItems");

            migrationBuilder.DropColumn(
                name: "CandidateFlightId",
                table: "DepItems");

            migrationBuilder.DropColumn(
                name: "DateOfFlight",
                table: "CandidateFlights");

            migrationBuilder.DropColumn(
                name: "DepItemId",
                table: "CandidateFlights");

            migrationBuilder.RenameColumn(
                name: "CvRefId",
                table: "CandidateFlights",
                newName: "CVRefId");

            migrationBuilder.RenameIndex(
                name: "IX_CandidateFlights_CvRefId",
                table: "CandidateFlights",
                newName: "IX_CandidateFlights_CVRefId");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ETD_Via",
                table: "CandidateFlightDetails",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "ETA_Via",
                table: "CandidateFlightDetails",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldNullable: true);
        }
    }
}

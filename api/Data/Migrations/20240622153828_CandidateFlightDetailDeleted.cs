using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Data.Migrations
{
    /// <inheritdoc />
    public partial class CandidateFlightDetailDeleted : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CandidateFlightDetails");

            migrationBuilder.AddColumn<string>(
                name: "AirportOfBoarding",
                table: "CandidateFlights",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AirportOfDestination",
                table: "CandidateFlights",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AirportVia",
                table: "CandidateFlights",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ETA_Destination",
                table: "CandidateFlights",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ETA_Via",
                table: "CandidateFlights",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ETD_Boarding",
                table: "CandidateFlights",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ETD_Via",
                table: "CandidateFlights",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FightNoVia",
                table: "CandidateFlights",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FlightNo",
                table: "CandidateFlights",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AirportOfBoarding",
                table: "CandidateFlights");

            migrationBuilder.DropColumn(
                name: "AirportOfDestination",
                table: "CandidateFlights");

            migrationBuilder.DropColumn(
                name: "AirportVia",
                table: "CandidateFlights");

            migrationBuilder.DropColumn(
                name: "ETA_Destination",
                table: "CandidateFlights");

            migrationBuilder.DropColumn(
                name: "ETA_Via",
                table: "CandidateFlights");

            migrationBuilder.DropColumn(
                name: "ETD_Boarding",
                table: "CandidateFlights");

            migrationBuilder.DropColumn(
                name: "ETD_Via",
                table: "CandidateFlights");

            migrationBuilder.DropColumn(
                name: "FightNoVia",
                table: "CandidateFlights");

            migrationBuilder.DropColumn(
                name: "FlightNo",
                table: "CandidateFlights");

            migrationBuilder.CreateTable(
                name: "CandidateFlightDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AirportOfBoarding = table.Column<string>(type: "TEXT", nullable: true),
                    AirportOfDestination = table.Column<string>(type: "TEXT", nullable: true),
                    AirportVia = table.Column<string>(type: "TEXT", nullable: true),
                    CandidateFlightId = table.Column<int>(type: "INTEGER", nullable: false),
                    ETA_Destination = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ETA_Via = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ETD_Boarding = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ETD_Via = table.Column<DateTime>(type: "TEXT", nullable: true),
                    FightNoVia = table.Column<string>(type: "TEXT", nullable: true),
                    FlightNo = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CandidateFlightDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CandidateFlightDetails_CandidateFlights_CandidateFlightId",
                        column: x => x.CandidateFlightId,
                        principalTable: "CandidateFlights",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CandidateFlightDetails_CandidateFlightId",
                table: "CandidateFlightDetails",
                column: "CandidateFlightId",
                unique: true);
        }
    }
}

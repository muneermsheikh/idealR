using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Data.Migrations
{
    /// <inheritdoc />
    public partial class ComposeMsgForCandidate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ProfessionName",
                table: "SelectionDecisions",
                newName: "SelectedAs");

            migrationBuilder.AddColumn<string>(
                name: "CandidateName",
                table: "SelectionDecisions",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Gender",
                table: "SelectionDecisions",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HrExecAssignedToUsername",
                table: "ContractReviews",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CandidateFlights",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CVRefId = table.Column<int>(type: "INTEGER", nullable: false),
                    ApplicationNo = table.Column<int>(type: "INTEGER", nullable: false),
                    CandidateName = table.Column<string>(type: "TEXT", nullable: true),
                    CustomerName = table.Column<string>(type: "TEXT", nullable: true),
                    CustomerCity = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CandidateFlights", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FlightDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FlightNo = table.Column<string>(type: "TEXT", nullable: true),
                    AirportOfBoarding = table.Column<string>(type: "TEXT", nullable: true),
                    AirportOfDestination = table.Column<string>(type: "TEXT", nullable: true),
                    AirportVia = table.Column<string>(type: "TEXT", nullable: true),
                    FightNoVia = table.Column<string>(type: "TEXT", nullable: true),
                    ETD_Boarding = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ETA_Destination = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ETA_Via = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ETD_Via = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FlightDetails", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CandidateFlightDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CandidateFlightId = table.Column<int>(type: "INTEGER", nullable: false),
                    FlightNo = table.Column<string>(type: "TEXT", nullable: true),
                    AirportOfBoarding = table.Column<string>(type: "TEXT", nullable: true),
                    AirportOfDestination = table.Column<string>(type: "TEXT", nullable: true),
                    AirportVia = table.Column<string>(type: "TEXT", nullable: true),
                    FightNoVia = table.Column<string>(type: "TEXT", nullable: true),
                    ETD_Boarding = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ETA_Destination = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ETA_Via = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ETD_Via = table.Column<DateTime>(type: "TEXT", nullable: false)
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

            migrationBuilder.CreateIndex(
                name: "IX_CandidateFlights_CVRefId",
                table: "CandidateFlights",
                column: "CVRefId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FlightDetails_FlightNo",
                table: "FlightDetails",
                column: "FlightNo",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CandidateFlightDetails");

            migrationBuilder.DropTable(
                name: "FlightDetails");

            migrationBuilder.DropTable(
                name: "CandidateFlights");

            migrationBuilder.DropColumn(
                name: "CandidateName",
                table: "SelectionDecisions");

            migrationBuilder.DropColumn(
                name: "Gender",
                table: "SelectionDecisions");

            migrationBuilder.DropColumn(
                name: "HrExecAssignedToUsername",
                table: "ContractReviews");

            migrationBuilder.RenameColumn(
                name: "SelectedAs",
                table: "SelectionDecisions",
                newName: "ProfessionName");
        }
    }
}

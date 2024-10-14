using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Data.Migrations
{
    /// <inheritdoc />
    public partial class flightItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CandidateFlights_CvRefId",
                table: "CandidateFlights");

            migrationBuilder.CreateTable(
                name: "CandidateFlightGrps",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateOfFlight = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OrderNo = table.Column<int>(type: "int", nullable: false),
                    AirlineName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FlightNo = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    AirportOfBoarding = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AirportOfDestination = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ETD_Boarding = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ETA_Destination = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AirportVia = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FightNoVia = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ETA_Via = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ETD_Via = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FullPath = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CandidateFlightGrps", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CandidateFlightItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CandidateFlightGrpId = table.Column<int>(type: "int", nullable: false),
                    DepId = table.Column<int>(type: "int", nullable: false),
                    CvRefId = table.Column<int>(type: "int", nullable: false),
                    DepItemId = table.Column<int>(type: "int", nullable: false),
                    ApplicationNo = table.Column<int>(type: "int", nullable: false),
                    CandidateName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerCity = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CandidateFlightItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CandidateFlightItems_CandidateFlightGrps_CandidateFlightGrpId",
                        column: x => x.CandidateFlightGrpId,
                        principalTable: "CandidateFlightGrps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CandidateFlightGrps_FlightNo",
                table: "CandidateFlightGrps",
                column: "FlightNo");

            migrationBuilder.CreateIndex(
                name: "IX_CandidateFlightItems_CandidateFlightGrpId",
                table: "CandidateFlightItems",
                column: "CandidateFlightGrpId");

            migrationBuilder.CreateIndex(
                name: "IX_CandidateFlightItems_CvRefId",
                table: "CandidateFlightItems",
                column: "CvRefId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CandidateFlightItems");

            migrationBuilder.DropTable(
                name: "CandidateFlightGrps");

            migrationBuilder.CreateIndex(
                name: "IX_CandidateFlights_CvRefId",
                table: "CandidateFlights",
                column: "CvRefId",
                unique: true);
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Data.Migrations
{
    /// <inheritdoc />
    public partial class FlightDataRemoved : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FlightDatas");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FlightDatas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AirportOfBoarding = table.Column<string>(type: "TEXT", nullable: true),
                    AirportOfDestination = table.Column<string>(type: "TEXT", nullable: true),
                    AirportVia = table.Column<string>(type: "TEXT", nullable: true),
                    ETA_Destination = table.Column<TimeOnly>(type: "TEXT", nullable: false),
                    ETA_Via = table.Column<TimeOnly>(type: "TEXT", nullable: false),
                    ETD_Boarding = table.Column<TimeOnly>(type: "TEXT", nullable: false),
                    ETD_Via = table.Column<TimeOnly>(type: "TEXT", nullable: false),
                    FlightNoVia = table.Column<string>(type: "TEXT", nullable: true),
                    flightNo = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FlightDatas", x => x.Id);
                });
        }
    }
}

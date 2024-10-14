using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Data.Migrations
{
    /// <inheritdoc />
    public partial class CandidateFlightGrpETDString : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ETA_DestinationString",
                table: "CandidateFlightGrps",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ETD_BoardingString",
                table: "CandidateFlightGrps",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ETA_DestinationString",
                table: "CandidateFlightGrps");

            migrationBuilder.DropColumn(
                name: "ETD_BoardingString",
                table: "CandidateFlightGrps");
        }
    }
}

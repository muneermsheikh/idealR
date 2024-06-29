using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Data.Migrations
{
    /// <inheritdoc />
    public partial class candidateflight_depitemsremoved : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DepItems_CandidateFlights_CandidateFlightId",
                table: "DepItems");

            migrationBuilder.DropIndex(
                name: "IX_DepItems_CandidateFlightId",
                table: "DepItems");

            migrationBuilder.DropColumn(
                name: "CandidateFlightId",
                table: "DepItems");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CandidateFlightId",
                table: "DepItems",
                type: "INTEGER",
                nullable: true);

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
    }
}

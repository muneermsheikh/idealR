using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Data.Migrations
{
    /// <inheritdoc />
    public partial class CandFltItemCVRefIdHasFilter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CandidateFlightItems_CvRefId",
                table: "CandidateFlightItems");

            migrationBuilder.CreateIndex(
                name: "IX_CandidateFlightItems_CvRefId",
                table: "CandidateFlightItems",
                column: "CvRefId",
                unique: true,
                filter: "CvRefId != 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CandidateFlightItems_CvRefId",
                table: "CandidateFlightItems");

            migrationBuilder.CreateIndex(
                name: "IX_CandidateFlightItems_CvRefId",
                table: "CandidateFlightItems",
                column: "CvRefId",
                unique: true);
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Data.Migrations
{
    /// <inheritdoc />
    public partial class InterviewItemCandidate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_IntervwItemCandidates_CandidateId_InterviewItemId",
                table: "IntervwItemCandidates");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_IntervwItemCandidates_CandidateId_InterviewItemId",
                table: "IntervwItemCandidates",
                columns: new[] { "CandidateId", "InterviewItemId" },
                unique: true,
                filter: "CandidateId <> 0");
        }
    }
}

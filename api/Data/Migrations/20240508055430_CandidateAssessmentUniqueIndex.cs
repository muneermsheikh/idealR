using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Data.Migrations
{
    /// <inheritdoc />
    public partial class CandidateAssessmentUniqueIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_CandidateAssessments_CandidateId_OrderItemId",
                table: "CandidateAssessments",
                columns: new[] { "CandidateId", "OrderItemId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CandidateAssessments_CandidateId_OrderItemId",
                table: "CandidateAssessments");
        }
    }
}

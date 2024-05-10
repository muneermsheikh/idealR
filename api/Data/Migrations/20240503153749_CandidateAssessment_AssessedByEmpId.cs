using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Data.Migrations
{
    /// <inheritdoc />
    public partial class CandidateAssessment_AssessedByEmpId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AssessedByEmployeeId",
                table: "CandidateAssessments");

            migrationBuilder.RenameColumn(
                name: "requireInternalReview",
                table: "CandidateAssessments",
                newName: "RequireInternalReview");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RequireInternalReview",
                table: "CandidateAssessments",
                newName: "requireInternalReview");

            migrationBuilder.AddColumn<int>(
                name: "AssessedByEmployeeId",
                table: "CandidateAssessments",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }
    }
}

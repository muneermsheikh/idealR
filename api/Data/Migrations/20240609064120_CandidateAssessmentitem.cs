using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Data.Migrations
{
    /// <inheritdoc />
    public partial class CandidateAssessmentitem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CandidatesItemAssessments_CandidateAssessments_CandidateAssessmentId",
                table: "CandidatesItemAssessments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CandidatesItemAssessments",
                table: "CandidatesItemAssessments");

            migrationBuilder.RenameTable(
                name: "CandidatesItemAssessments",
                newName: "CandidatesAssessmentItems");

            migrationBuilder.RenameIndex(
                name: "IX_CandidatesItemAssessments_CandidateAssessmentId",
                table: "CandidatesAssessmentItems",
                newName: "IX_CandidatesAssessmentItems_CandidateAssessmentId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CandidatesAssessmentItems",
                table: "CandidatesAssessmentItems",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CandidatesAssessmentItems_CandidateAssessments_CandidateAssessmentId",
                table: "CandidatesAssessmentItems",
                column: "CandidateAssessmentId",
                principalTable: "CandidateAssessments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CandidatesAssessmentItems_CandidateAssessments_CandidateAssessmentId",
                table: "CandidatesAssessmentItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CandidatesAssessmentItems",
                table: "CandidatesAssessmentItems");

            migrationBuilder.RenameTable(
                name: "CandidatesAssessmentItems",
                newName: "CandidatesItemAssessments");

            migrationBuilder.RenameIndex(
                name: "IX_CandidatesAssessmentItems_CandidateAssessmentId",
                table: "CandidatesItemAssessments",
                newName: "IX_CandidatesItemAssessments_CandidateAssessmentId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CandidatesItemAssessments",
                table: "CandidatesItemAssessments",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CandidatesItemAssessments_CandidateAssessments_CandidateAssessmentId",
                table: "CandidatesItemAssessments",
                column: "CandidateAssessmentId",
                principalTable: "CandidateAssessments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Data.Migrations
{
    /// <inheritdoc />
    public partial class checklist_salaries : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChecklistHRs_Candidates_CandidateId",
                table: "ChecklistHRs");

            migrationBuilder.DropForeignKey(
                name: "FK_ChecklistHRs_OrderItems_OrderItemId",
                table: "ChecklistHRs");

            migrationBuilder.DropIndex(
                name: "IX_ChecklistHRs_CandidateId",
                table: "ChecklistHRs");

            migrationBuilder.AddColumn<int>(
                name: "SalaryExpectation",
                table: "ChecklistHRs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SalaryOffered",
                table: "ChecklistHRs",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SalaryExpectation",
                table: "ChecklistHRs");

            migrationBuilder.DropColumn(
                name: "SalaryOffered",
                table: "ChecklistHRs");

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistHRs_CandidateId",
                table: "ChecklistHRs",
                column: "CandidateId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChecklistHRs_Candidates_CandidateId",
                table: "ChecklistHRs",
                column: "CandidateId",
                principalTable: "Candidates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChecklistHRs_OrderItems_OrderItemId",
                table: "ChecklistHRs",
                column: "OrderItemId",
                principalTable: "OrderItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

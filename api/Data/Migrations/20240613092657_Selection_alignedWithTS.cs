using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Data.Migrations
{
    /// <inheritdoc />
    public partial class Selection_alignedWithTS : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RejectionReason",
                table: "SelectionDecisions",
                newName: "CustomerName");

            migrationBuilder.AddColumn<int>(
                name: "ApplicationNo",
                table: "SelectionDecisions",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CandidateId",
                table: "SelectionDecisions",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApplicationNo",
                table: "SelectionDecisions");

            migrationBuilder.DropColumn(
                name: "CandidateId",
                table: "SelectionDecisions");

            migrationBuilder.RenameColumn(
                name: "CustomerName",
                table: "SelectionDecisions",
                newName: "RejectionReason");
        }
    }
}

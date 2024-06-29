using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Data.Migrations
{
    /// <inheritdoc />
    public partial class SelectionCVRefId_CvRefId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SelectionDecisions_CVRefs_CVRefId",
                table: "SelectionDecisions");

            migrationBuilder.RenameColumn(
                name: "CVRefId",
                table: "SelectionDecisions",
                newName: "CvRefId");

            migrationBuilder.RenameIndex(
                name: "IX_SelectionDecisions_CVRefId",
                table: "SelectionDecisions",
                newName: "IX_SelectionDecisions_CvRefId");

            migrationBuilder.AddForeignKey(
                name: "FK_SelectionDecisions_CVRefs_CvRefId",
                table: "SelectionDecisions",
                column: "CvRefId",
                principalTable: "CVRefs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SelectionDecisions_CVRefs_CvRefId",
                table: "SelectionDecisions");

            migrationBuilder.RenameColumn(
                name: "CvRefId",
                table: "SelectionDecisions",
                newName: "CVRefId");

            migrationBuilder.RenameIndex(
                name: "IX_SelectionDecisions_CvRefId",
                table: "SelectionDecisions",
                newName: "IX_SelectionDecisions_CVRefId");

            migrationBuilder.AddForeignKey(
                name: "FK_SelectionDecisions_CVRefs_CVRefId",
                table: "SelectionDecisions",
                column: "CVRefId",
                principalTable: "CVRefs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

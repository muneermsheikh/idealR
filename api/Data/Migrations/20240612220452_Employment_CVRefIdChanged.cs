using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Data.Migrations
{
    /// <inheritdoc />
    public partial class Employment_CVRefIdChanged : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CVRefId",
                table: "Employments",
                newName: "CvRefId");

            migrationBuilder.RenameIndex(
                name: "IX_Employments_CVRefId",
                table: "Employments",
                newName: "IX_Employments_CvRefId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CvRefId",
                table: "Employments",
                newName: "CVRefId");

            migrationBuilder.RenameIndex(
                name: "IX_Employments_CvRefId",
                table: "Employments",
                newName: "IX_Employments_CVRefId");
        }
    }
}

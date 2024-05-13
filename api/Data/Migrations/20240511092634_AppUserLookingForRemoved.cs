using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Data.Migrations
{
    /// <inheritdoc />
    public partial class AppUserLookingForRemoved : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SelectionDecisions_CVRefId",
                table: "SelectionDecisions");

            migrationBuilder.DropColumn(
                name: "Introduction",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "LookingFor",
                table: "AspNetUsers");

            migrationBuilder.CreateIndex(
                name: "IX_SelectionDecisions_CVRefId",
                table: "SelectionDecisions",
                column: "CVRefId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employments_CVRefId",
                table: "Employments",
                column: "CVRefId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SelectionDecisions_CVRefId",
                table: "SelectionDecisions");

            migrationBuilder.DropIndex(
                name: "IX_Employments_CVRefId",
                table: "Employments");

            migrationBuilder.AddColumn<string>(
                name: "Introduction",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LookingFor",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SelectionDecisions_CVRefId",
                table: "SelectionDecisions",
                column: "CVRefId");
        }
    }
}

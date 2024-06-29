using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Data.Migrations
{
    /// <inheritdoc />
    public partial class SelDecCVRefId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Deps_CVRefs_CVRefId",
                table: "Deps");

            migrationBuilder.RenameColumn(
                name: "CVRefId",
                table: "Deps",
                newName: "CvRefId");

            migrationBuilder.RenameIndex(
                name: "IX_Deps_CVRefId",
                table: "Deps",
                newName: "IX_Deps_CvRefId");

            migrationBuilder.AddColumn<bool>(
                name: "Ecnr",
                table: "Deps",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_Deps_CVRefs_CvRefId",
                table: "Deps",
                column: "CvRefId",
                principalTable: "CVRefs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Deps_CVRefs_CvRefId",
                table: "Deps");

            migrationBuilder.DropColumn(
                name: "Ecnr",
                table: "Deps");

            migrationBuilder.RenameColumn(
                name: "CvRefId",
                table: "Deps",
                newName: "CVRefId");

            migrationBuilder.RenameIndex(
                name: "IX_Deps_CvRefId",
                table: "Deps",
                newName: "IX_Deps_CVRefId");

            migrationBuilder.AddForeignKey(
                name: "FK_Deps_CVRefs_CVRefId",
                table: "Deps",
                column: "CVRefId",
                principalTable: "CVRefs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

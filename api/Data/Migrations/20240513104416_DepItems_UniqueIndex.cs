using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Data.Migrations
{
    /// <inheritdoc />
    public partial class DepItems_UniqueIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Deps_CVRefId",
                table: "Deps");

            migrationBuilder.DropIndex(
                name: "IX_DepItems_DepId",
                table: "DepItems");

            migrationBuilder.CreateIndex(
                name: "IX_Deps_CVRefId",
                table: "Deps",
                column: "CVRefId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DepItems_DepId_Sequence",
                table: "DepItems",
                columns: new[] { "DepId", "Sequence" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Deps_CVRefId",
                table: "Deps");

            migrationBuilder.DropIndex(
                name: "IX_DepItems_DepId_Sequence",
                table: "DepItems");

            migrationBuilder.CreateIndex(
                name: "IX_Deps_CVRefId",
                table: "Deps",
                column: "CVRefId");

            migrationBuilder.CreateIndex(
                name: "IX_DepItems_DepId",
                table: "DepItems",
                column: "DepId");
        }
    }
}

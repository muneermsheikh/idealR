using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Data.Migrations
{
    /// <inheritdoc />
    public partial class InterviewItemCandt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "IntervwItemId",
                table: "IntervwItemCandidates",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_IntervwItemCandidates_CandidateId_IntervwItemId",
                table: "IntervwItemCandidates",
                columns: new[] { "CandidateId", "IntervwItemId" },
                unique: true,
                filter: "CandidateId <> 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_IntervwItemCandidates_CandidateId_IntervwItemId",
                table: "IntervwItemCandidates");

            migrationBuilder.AlterColumn<int>(
                name: "IntervwItemId",
                table: "IntervwItemCandidates",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}

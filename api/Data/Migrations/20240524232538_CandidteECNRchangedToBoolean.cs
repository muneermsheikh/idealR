using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Data.Migrations
{
    /// <inheritdoc />
    public partial class CandidteECNRchangedToBoolean : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Ecnr",
                table: "Candidates",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "INTEGER");

            migrationBuilder.CreateIndex(
                name: "IX_UserQualifications_CandidateId",
                table: "UserQualifications",
                column: "CandidateId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserQualifications_Candidates_CandidateId",
                table: "UserQualifications",
                column: "CandidateId",
                principalTable: "Candidates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserQualifications_Candidates_CandidateId",
                table: "UserQualifications");

            migrationBuilder.DropIndex(
                name: "IX_UserQualifications_CandidateId",
                table: "UserQualifications");

            migrationBuilder.AlterColumn<bool>(
                name: "Ecnr",
                table: "Candidates",
                type: "INTEGER",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);
        }
    }
}
